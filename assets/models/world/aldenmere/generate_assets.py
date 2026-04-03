"""
DataSoul — Aldenmere Assets Generator
Generates GLB files for the starting town Aldenmere using pygltflib + numpy.
Art Style: Dark anime, medieval, .hack reference
"""

import numpy as np
import struct
import json
import base64
import os

# Output directory
OUT_DIR = os.path.dirname(os.path.abspath(__file__))

# ─── GLB Builder ─────────────────────────────────────────────────────────────

def pack_f32(values):
    return struct.pack(f"{len(values)}f", *values)

def pack_u16(values):
    return struct.pack(f"{len(values)}H", *values)

def pack_u32(values):
    return struct.pack(f"{len(values)}I", *values)

def align4(data: bytes) -> bytes:
    rem = len(data) % 4
    if rem:
        data += b"\x00" * (4 - rem)
    return data

class GLBBuilder:
    """Minimal GLB builder — constructs a valid GLB 2.0 file."""

    def __init__(self):
        self.meshes = []        # list of mesh dicts
        self.nodes = []         # list of node dicts
        self.scene_nodes = []   # top-level node indices
        self.materials = []     # list of material dicts
        self._bin_chunks = []   # raw binary data blobs
        self._bin_offset = 0
        self._accessors = []
        self._buffer_views = []

    # ── Low-level data append ──────────────────────────────────────────────

    def _add_bin(self, data: bytes):
        data = align4(data)
        view_idx = len(self._buffer_views)
        self._buffer_views.append({
            "buffer": 0,
            "byteOffset": self._bin_offset,
            "byteLength": len(data),
        })
        self._bin_chunks.append(data)
        self._bin_offset += len(data)
        return view_idx

    def _add_accessor_vec3(self, data_f32: np.ndarray, is_indices=False):
        """Add a VEC3 float32 accessor."""
        flat = data_f32.astype(np.float32).flatten().tolist()
        raw = pack_f32(flat)
        view_idx = self._add_bin(raw)
        mn = data_f32.min(axis=0).tolist()
        mx = data_f32.max(axis=0).tolist()
        acc = {
            "bufferView": view_idx,
            "componentType": 5126,  # FLOAT
            "count": len(data_f32),
            "type": "VEC3",
            "min": mn,
            "max": mx,
        }
        idx = len(self._accessors)
        self._accessors.append(acc)
        return idx

    def _add_accessor_scalar_u32(self, indices: list):
        """Add a SCALAR UINT32 accessor for indices."""
        raw = pack_u32(indices)
        view_idx = self._add_bin(raw)
        acc = {
            "bufferView": view_idx,
            "componentType": 5125,  # UNSIGNED_INT
            "count": len(indices),
            "type": "SCALAR",
        }
        idx = len(self._accessors)
        self._accessors.append(acc)
        return idx

    def _add_accessor_vec2(self, data_f32: np.ndarray):
        """Add a VEC2 float32 accessor."""
        flat = data_f32.astype(np.float32).flatten().tolist()
        raw = pack_f32(flat)
        view_idx = self._add_bin(raw)
        acc = {
            "bufferView": view_idx,
            "componentType": 5126,
            "count": len(data_f32),
            "type": "VEC2",
        }
        idx = len(self._accessors)
        self._accessors.append(acc)
        return idx

    # ── Material ──────────────────────────────────────────────────────────

    def add_material(self, name, r, g, b, a=1.0, roughness=0.7, metallic=0.0, emissive=None):
        mat = {
            "name": name,
            "pbrMetallicRoughness": {
                "baseColorFactor": [r, g, b, a],
                "metallicFactor": metallic,
                "roughnessFactor": roughness,
            },
            "doubleSided": False,
        }
        if emissive:
            mat["emissiveFactor"] = emissive
        idx = len(self.materials)
        self.materials.append(mat)
        return idx

    # ── Geometry helpers ──────────────────────────────────────────────────

    def _box_geometry(self, sx, sy, sz, cx=0, cy=0, cz=0):
        """Returns (positions, normals, uvs, indices) for a box."""
        x, y, z = cx, cy, cz
        hx, hy, hz = sx/2, sy/2, sz/2

        # 6 faces, 4 verts each, 2 triangles each
        positions = []
        normals = []
        uvs = []
        indices = []

        faces = [
            # (normal, corners)
            ([0, 0, 1],  [[-hx,-hy, hz],[ hx,-hy, hz],[ hx, hy, hz],[-hx, hy, hz]]),
            ([0, 0,-1],  [[ hx,-hy,-hz],[-hx,-hy,-hz],[-hx, hy,-hz],[ hx, hy,-hz]]),
            ([0, 1, 0],  [[-hx, hy, hz],[ hx, hy, hz],[ hx, hy,-hz],[-hx, hy,-hz]]),
            ([0,-1, 0],  [[-hx,-hy,-hz],[ hx,-hy,-hz],[ hx,-hy, hz],[-hx,-hy, hz]]),
            ([1, 0, 0],  [[ hx,-hy, hz],[ hx,-hy,-hz],[ hx, hy,-hz],[ hx, hy, hz]]),
            ([-1, 0, 0], [[-hx,-hy,-hz],[-hx,-hy, hz],[-hx, hy, hz],[-hx, hy,-hz]]),
        ]

        for normal, corners in faces:
            base = len(positions)
            for c in corners:
                positions.append([c[0]+x, c[1]+y, c[2]+z])
                normals.append(normal)
            uvs.extend([[0,0],[1,0],[1,1],[0,1]])
            indices.extend([base, base+1, base+2, base, base+2, base+3])

        return (np.array(positions, dtype=np.float32),
                np.array(normals, dtype=np.float32),
                np.array(uvs, dtype=np.float32),
                indices)

    def _cylinder_geometry(self, radius, height, segments=12, cx=0, cy=0, cz=0):
        """Returns (positions, normals, uvs, indices) for a cylinder."""
        positions = []
        normals = []
        uvs = []
        indices = []

        angles = [2*np.pi * i / segments for i in range(segments)]

        # Side faces
        for i in range(segments):
            a0 = angles[i]
            a1 = angles[(i+1) % segments]
            x0, z0 = radius * np.cos(a0), radius * np.sin(a0)
            x1, z1 = radius * np.cos(a1), radius * np.sin(a1)
            base = len(positions)
            positions.extend([
                [cx+x0, cy,        cz+z0],
                [cx+x1, cy,        cz+z1],
                [cx+x1, cy+height, cz+z1],
                [cx+x0, cy+height, cz+z0],
            ])
            # Approximate normals
            nx0 = np.cos((a0+a1)/2)
            nz0 = np.sin((a0+a1)/2)
            for _ in range(4):
                normals.append([nx0, 0, nz0])
            uvs.extend([[i/segments, 0],[( i+1)/segments, 0],
                        [(i+1)/segments, 1],[i/segments, 1]])
            indices.extend([base, base+1, base+2, base, base+2, base+3])

        # Top cap
        center_top = len(positions)
        positions.append([cx, cy+height, cz])
        normals.append([0, 1, 0])
        uvs.append([0.5, 0.5])
        for i in range(segments):
            a = angles[i]
            positions.append([cx + radius*np.cos(a), cy+height, cz + radius*np.sin(a)])
            normals.append([0, 1, 0])
            uvs.append([0.5+0.5*np.cos(a), 0.5+0.5*np.sin(a)])
        for i in range(segments):
            indices.extend([center_top, center_top+1+i, center_top+1+(i+1)%segments])

        # Bottom cap
        center_bot = len(positions)
        positions.append([cx, cy, cz])
        normals.append([0,-1,0])
        uvs.append([0.5,0.5])
        for i in range(segments):
            a = angles[i]
            positions.append([cx + radius*np.cos(a), cy, cz + radius*np.sin(a)])
            normals.append([0,-1,0])
            uvs.append([0.5+0.5*np.cos(a), 0.5+0.5*np.sin(a)])
        for i in range(segments):
            indices.extend([center_bot, center_bot+1+(i+1)%segments, center_bot+1+i])

        return (np.array(positions, dtype=np.float32),
                np.array(normals, dtype=np.float32),
                np.array(uvs, dtype=np.float32),
                indices)

    def _plane_geometry(self, width, depth, cx=0, cy=0, cz=0, subdivisions=1):
        """Returns (positions, normals, uvs, indices) for a flat XZ plane."""
        positions = []
        normals = []
        uvs = []
        indices = []

        s = subdivisions
        for xi in range(s):
            for zi in range(s):
                x0 = cx - width/2 + xi*(width/s)
                x1 = x0 + width/s
                z0 = cz - depth/2 + zi*(depth/s)
                z1 = z0 + depth/s
                base = len(positions)
                positions.extend([[x0,cy,z0],[x1,cy,z0],[x1,cy,z1],[x0,cy,z1]])
                for _ in range(4): normals.append([0,1,0])
                u0, u1 = xi/s, (xi+1)/s
                v0, v1 = zi/s, (zi+1)/s
                uvs.extend([[u0,v0],[u1,v0],[u1,v1],[u0,v1]])
                indices.extend([base, base+1, base+2, base, base+2, base+3])

        return (np.array(positions, dtype=np.float32),
                np.array(normals, dtype=np.float32),
                np.array(uvs, dtype=np.float32),
                indices)

    def _merge_geo(self, geos):
        """Merge multiple geometry tuples (pos, nrm, uv, idx) into one."""
        all_pos, all_nrm, all_uv, all_idx = [], [], [], []
        offset = 0
        for pos, nrm, uv, idx in geos:
            all_pos.append(pos)
            all_nrm.append(nrm)
            all_uv.append(uv)
            all_idx.extend([i + offset for i in idx])
            offset += len(pos)
        return (np.vstack(all_pos), np.vstack(all_nrm),
                np.vstack(all_uv), all_idx)

    # ── Mesh / Node creation ──────────────────────────────────────────────

    def add_mesh_from_geo(self, name, pos, nrm, uv, idx, mat_idx):
        """Add a mesh from geometry arrays."""
        pos_acc = self._add_accessor_vec3(pos)
        nrm_acc = self._add_accessor_vec3(nrm)
        uv_acc  = self._add_accessor_vec2(uv)
        idx_acc = self._add_accessor_scalar_u32(idx)

        mesh = {
            "name": name,
            "primitives": [{
                "attributes": {
                    "POSITION": pos_acc,
                    "NORMAL": nrm_acc,
                    "TEXCOORD_0": uv_acc,
                },
                "indices": idx_acc,
                "material": mat_idx,
            }]
        }
        mesh_idx = len(self.meshes)
        self.meshes.append(mesh)
        return mesh_idx

    def add_node(self, name, mesh_idx, translation=None, rotation=None, scale=None, children=None):
        node = {"name": name, "mesh": mesh_idx}
        if translation: node["translation"] = translation
        if rotation:    node["rotation"] = rotation
        if scale:       node["scale"] = scale
        if children:    node["children"] = children
        idx = len(self.nodes)
        self.nodes.append(node)
        return idx

    def add_empty_node(self, name, translation=None, children=None):
        node = {"name": name}
        if translation: node["translation"] = translation
        if children:    node["children"] = children
        idx = len(self.nodes)
        self.nodes.append(node)
        return idx

    # ── GLB export ────────────────────────────────────────────────────────

    def build(self, filepath):
        bin_data = b"".join(self._bin_chunks)
        # align bin chunk to 4 bytes
        pad = (4 - len(bin_data) % 4) % 4
        bin_data += b"\x00" * pad

        gltf = {
            "asset": {"version": "2.0", "generator": "DataSoul-AssetGen"},
            "scene": 0,
            "scenes": [{"name": "Scene", "nodes": self.scene_nodes}],
            "nodes": self.nodes,
            "meshes": self.meshes,
            "materials": self.materials,
            "accessors": self._accessors,
            "bufferViews": self._buffer_views,
            "buffers": [{"byteLength": len(bin_data)}],
        }

        json_bytes = json.dumps(gltf, separators=(',', ':')).encode("utf-8")
        json_pad = (4 - len(json_bytes) % 4) % 4
        json_bytes += b" " * json_pad

        # GLB header: magic, version, total length
        total = 12 + 8 + len(json_bytes) + 8 + len(bin_data)
        header = struct.pack("<III", 0x46546C67, 2, total)

        chunk_json = struct.pack("<II", len(json_bytes), 0x4E4F534A) + json_bytes
        chunk_bin  = struct.pack("<II", len(bin_data),  0x004E4942) + bin_data

        with open(filepath, "wb") as f:
            f.write(header + chunk_json + chunk_bin)
        print(f"  ✓ {os.path.basename(filepath)} ({os.path.getsize(filepath)//1024} KB)")


# ═════════════════════════════════════════════════════════════════════════════
# MODEL 1 — aldenmere_main_square.glb
# ═════════════════════════════════════════════════════════════════════════════

def make_main_square():
    g = GLBBuilder()

    # Materials
    stone_dark  = g.add_material("Stone_Dark",      0.25, 0.22, 0.20, roughness=0.9)
    stone_pave  = g.add_material("Stone_Pavement",  0.35, 0.32, 0.28, roughness=0.85)
    water_mat   = g.add_material("Water_Fountain",  0.10, 0.25, 0.45, roughness=0.2, metallic=0.1)
    iron_mat    = g.add_material("Iron_Lantern",    0.15, 0.15, 0.15, roughness=0.6, metallic=0.4)
    glow_mat    = g.add_material("Lantern_Glow",    1.00, 0.85, 0.40, roughness=0.9,
                                  emissive=[1.0, 0.85, 0.4])
    rune_mat    = g.add_material("Rune_Gold",       0.80, 0.65, 0.10, roughness=0.3, metallic=0.7,
                                  emissive=[0.3, 0.25, 0.0])
    wood_mat    = g.add_material("Wood_Market",     0.40, 0.28, 0.15, roughness=0.9)

    # ── Ground: paved square 30×30 ────────────────────────────────────────
    gp, gn, gu, gi = g._plane_geometry(30, 30, subdivisions=6)
    ground_m = g.add_mesh_from_geo("Ground", gp, gn, gu, gi, stone_pave)
    ground_n = g.add_node("Ground", ground_m)

    # Collision for ground
    cp, cn, cu, ci = g._plane_geometry(30, 30)
    col_m = g.add_mesh_from_geo("Ground-col", cp, cn, cu, ci, stone_pave)
    col_n = g.add_node("Ground-col", col_m)

    # ── Central fountain ──────────────────────────────────────────────────
    # Basin outer ring (cylinder)
    bp, bn, bu, bi = g._cylinder_geometry(2.2, 0.5, cx=0, cy=0, cz=0)
    basin_m = g.add_mesh_from_geo("Fountain_Basin", bp, bn, bu, bi, stone_dark)
    basin_n = g.add_node("Fountain_Basin", basin_m)

    # Basin inner (water surface)
    wp, wn, wu, wi = g._plane_geometry(3.6, 3.6, cy=0.48)
    water_m = g.add_mesh_from_geo("Fountain_Water", wp, wn, wu, wi, water_mat)
    water_n = g.add_node("Fountain_Water", water_m)

    # Center pillar
    pp, pn, pu, pi = g._cylinder_geometry(0.18, 1.8, cx=0, cy=0.5, cz=0)
    pillar_m = g.add_mesh_from_geo("Fountain_Pillar", pp, pn, pu, pi, stone_dark)
    pillar_n = g.add_node("Fountain_Pillar", pillar_m)

    # Top bowl
    tbp, tbn, tbu, tbi = g._cylinder_geometry(0.5, 0.25, cx=0, cy=2.3, cz=0)
    tbowl_m = g.add_mesh_from_geo("Fountain_TopBowl", tbp, tbn, tbu, tbi, stone_dark)
    tbowl_n = g.add_node("Fountain_TopBowl", tbowl_m)

    # Collision for fountain
    fcp, fcn, fcu, fci = g._cylinder_geometry(2.3, 0.6)
    fc_m = g.add_mesh_from_geo("Fountain-col", fcp, fcn, fcu, fci, stone_dark)
    fc_n = g.add_node("Fountain-col", fc_m)

    # ── Hexagonal rune ring on ground (6 small flat hexagons) ─────────────
    rune_nodes = []
    for i in range(6):
        angle = i * np.pi / 3
        rx = 3.5 * np.cos(angle)
        rz = 3.5 * np.sin(angle)
        rp, rn, ru, ri = g._plane_geometry(0.4, 0.4, cx=rx, cy=0.01, cz=rz)
        rm = g.add_mesh_from_geo(f"Rune_{i}", rp, rn, ru, ri, rune_mat)
        rn_ = g.add_node(f"Rune_{i}", rm)
        rune_nodes.append(rn_)

    # ── Lanterns (4 corners of square) ────────────────────────────────────
    lantern_nodes = []
    for lx, lz in [(-11, -11), (11, -11), (-11, 11), (11, 11)]:
        # Post
        pp2, pn2, pu2, pi2 = g._cylinder_geometry(0.08, 3.5, cx=lx, cy=0, cz=lz)
        pm2 = g.add_mesh_from_geo(f"LanternPost_{lx}_{lz}", pp2, pn2, pu2, pi2, iron_mat)
        pn2_ = g.add_node(f"LanternPost_{lx}_{lz}", pm2)
        # Light box
        lbp, lbn, lbu, lbi = g._box_geometry(0.3, 0.4, 0.3, cx=lx, cy=3.5, cz=lz)
        lbm = g.add_mesh_from_geo(f"LanternBox_{lx}_{lz}", lbp, lbn, lbu, lbi, iron_mat)
        lbn_ = g.add_node(f"LanternBox_{lx}_{lz}", lbm)
        # Glow
        gp2, gn2, gu2, gi2 = g._box_geometry(0.2, 0.25, 0.2, cx=lx, cy=3.55, cz=lz)
        gm2 = g.add_mesh_from_geo(f"LanternGlow_{lx}_{lz}", gp2, gn2, gu2, gi2, glow_mat)
        gn2_ = g.add_node(f"LanternGlow_{lx}_{lz}", gm2)
        lantern_nodes.extend([pn2_, lbn_, gn2_])

    # ── Market stalls (2 simple canopy+table) ─────────────────────────────
    market_nodes = []
    for mx, mz in [(-7, -6), (7, -6)]:
        # Table
        tp, tn, tu, ti = g._box_geometry(2.5, 0.1, 1.2, cx=mx, cy=0.8, cz=mz)
        tm_ = g.add_mesh_from_geo(f"Table_{mx}_{mz}", tp, tn, tu, ti, wood_mat)
        tn_ = g.add_node(f"Table_{mx}_{mz}", tm_)
        # Legs (4)
        leg_nodes = []
        for lx2, lz2 in [(-1.1, -0.5), (1.1, -0.5), (-1.1, 0.5), (1.1, 0.5)]:
            lp, ln, lu, li = g._box_geometry(0.08, 0.8, 0.08, cx=mx+lx2, cy=0.4, cz=mz+lz2)
            lm = g.add_mesh_from_geo(f"Leg_{mx}_{mz}_{lx2}_{lz2}", lp, ln, lu, li, wood_mat)
            ln_ = g.add_node(f"Leg_{mx}_{mz}_{lx2}_{lz2}", lm)
            leg_nodes.append(ln_)
        # Canopy
        cap, can, cau, cai = g._box_geometry(3.0, 0.08, 1.6, cx=mx, cy=2.2, cz=mz)
        cam = g.add_mesh_from_geo(f"Canopy_{mx}_{mz}", cap, can, cau, cai, stone_pave)
        can_ = g.add_node(f"Canopy_{mx}_{mz}", cam)
        market_nodes.extend([tn_] + leg_nodes + [can_])

    # Assemble scene
    all_nodes = ([ground_n, col_n, basin_n, water_n, pillar_n, tbowl_n, fc_n]
                 + rune_nodes + lantern_nodes + market_nodes)
    g.scene_nodes = all_nodes
    g.build(os.path.join(OUT_DIR, "aldenmere_main_square.glb"))


# ═════════════════════════════════════════════════════════════════════════════
# MODEL 2 — akademie_exterior.glb
# ═════════════════════════════════════════════════════════════════════════════

def make_akademie_exterior():
    g = GLBBuilder()

    stone_gray  = g.add_material("Stone_Gray",    0.30, 0.28, 0.25, roughness=0.88)
    stone_dark  = g.add_material("Stone_Dark",    0.18, 0.16, 0.14, roughness=0.9)
    rune_gold   = g.add_material("Rune_Gold",     0.82, 0.67, 0.12, roughness=0.3, metallic=0.8,
                                   emissive=[0.25, 0.20, 0.0])
    roof_mat    = g.add_material("Roof_Slate",    0.20, 0.20, 0.22, roughness=0.85)
    window_mat  = g.add_material("Window_Blue",   0.05, 0.10, 0.30, roughness=0.1, metallic=0.2,
                                   emissive=[0.0, 0.05, 0.20])

    # Ground base
    gp, gn, gu, gi = g._plane_geometry(28, 20)
    gm = g.add_mesh_from_geo("Ground", gp, gn, gu, gi, stone_gray)
    ground_n = g.add_node("Ground", gm)

    # ── Main building body ────────────────────────────────────────────────
    bp, bn, bu, bi = g._box_geometry(20, 12, 10, cy=6)
    bm = g.add_mesh_from_geo("MainBody", bp, bn, bu, bi, stone_gray)
    body_n = g.add_node("MainBody", bm)

    # Collision for main body
    cp, cn, cu, ci = g._box_geometry(20, 12, 10, cy=6)
    cm = g.add_mesh_from_geo("MainBody-col", cp, cn, cu, ci, stone_gray)
    col_n = g.add_node("MainBody-col", cm)

    # ── Two tall towers (sides) ───────────────────────────────────────────
    tower_nodes = []
    for tx in [-11, 11]:
        # Tower body
        tp, tn, tu, ti = g._cylinder_geometry(2.2, 18, 10, cx=tx, cy=0)
        tm = g.add_mesh_from_geo(f"Tower_{tx}", tp, tn, tu, ti, stone_gray)
        tn_ = g.add_node(f"Tower_{tx}", tm)
        # Tower top (conical via narrow box)
        top_p, top_n, top_u, top_i = g._cylinder_geometry(2.4, 0.5, 10, cx=tx, cy=18)
        top_m = g.add_mesh_from_geo(f"TowerRing_{tx}", top_p, top_n, top_u, top_i, stone_dark)
        top_n_ = g.add_node(f"TowerRing_{tx}", top_m)
        # Spire
        sp, sn, su, si = g._cylinder_geometry(0.25, 5, 8, cx=tx, cy=18.5)
        sm = g.add_mesh_from_geo(f"TowerSpire_{tx}", sp, sn, su, si, roof_mat)
        sn_ = g.add_node(f"TowerSpire_{tx}", sm)
        # Tower collision
        tcp, tcn, tcu, tci = g._cylinder_geometry(2.2, 18, 10, cx=tx)
        tcm = g.add_mesh_from_geo(f"Tower_{tx}-col", tcp, tcn, tcu, tci, stone_gray)
        tcn_ = g.add_node(f"Tower_{tx}-col", tcm)
        tower_nodes.extend([tn_, top_n_, sn_, tcn_])

    # ── Central entrance ──────────────────────────────────────────────────
    # Arch top (box approximation)
    ap, an, au, ai = g._box_geometry(3, 5, 1.2, cy=2.5, cz=5.1)
    am = g.add_mesh_from_geo("Arch", ap, an, au, ai, stone_dark)
    arch_n = g.add_node("Arch", am)

    # Large doorway (recessed dark box)
    dp, dn, du, di = g._box_geometry(2.0, 4.0, 0.5, cy=2.0, cz=5.3)
    dm = g.add_mesh_from_geo("Door_Opening", dp, dn, du, di, stone_dark)
    door_n = g.add_node("Door_Opening", dm)

    # ── Windows (rows on facade) ──────────────────────────────────────────
    win_nodes = []
    for wx in [-7, -3.5, 0, 3.5, 7]:
        for wy in [5, 9]:
            wp, wn, wu, wi = g._box_geometry(0.9, 1.5, 0.12, cx=wx, cy=wy, cz=5.06)
            wm = g.add_mesh_from_geo(f"Win_{wx}_{wy}", wp, wn, wu, wi, window_mat)
            wn_ = g.add_node(f"Win_{wx}_{wy}", wm)
            win_nodes.append(wn_)

    # Tower windows
    for tx in [-11, 11]:
        for wy in [5, 10, 14]:
            wp, wn, wu, wi = g._box_geometry(0.6, 0.9, 0.12,
                                               cx=tx, cy=wy, cz=2.1)
            wm = g.add_mesh_from_geo(f"TowerWin_{tx}_{wy}", wp, wn, wu, wi, window_mat)
            wn_ = g.add_node(f"TowerWin_{tx}_{wy}", wm)
            win_nodes.append(wn_)

    # ── Hexagonal rune decorations on facade ─────────────────────────────
    rune_nodes = []
    for rx, ry in [(-8, 3), (8, 3), (-5, 8), (5, 8), (0, 11), (-8, 8), (8, 8)]:
        rp, rn, ru, ri = g._box_geometry(0.5, 0.5, 0.08, cx=rx, cy=ry, cz=5.05)
        rm = g.add_mesh_from_geo(f"FacadeRune_{rx}_{ry}", rp, rn, ru, ri, rune_gold)
        rn_ = g.add_node(f"FacadeRune_{rx}_{ry}", rm)
        rune_nodes.append(rn_)

    # Roof
    roof_p, roof_n, roof_u, roof_i = g._box_geometry(22, 0.8, 12, cy=12.4)
    roof_m = g.add_mesh_from_geo("Roof", roof_p, roof_n, roof_u, roof_i, roof_mat)
    roof_n_ = g.add_node("Roof", roof_m)

    # Steps leading to entrance
    step_nodes = []
    for si, (sh, sz) in enumerate([(0.3, 5.8), (0.6, 6.2), (0.9, 6.6)]):
        sw = 5.0 - si * 0.5
        sp, sn, su, si2 = g._box_geometry(sw, 0.3, 0.4, cy=sh, cz=sz)
        sm = g.add_mesh_from_geo(f"Step_{si}", sp, sn, su, si2, stone_dark)
        sn_ = g.add_node(f"Step_{si}", sm)
        step_nodes.append(sn_)

    all_nodes = ([ground_n, body_n, col_n, arch_n, door_n, roof_n_]
                 + tower_nodes + win_nodes + rune_nodes + step_nodes)
    g.scene_nodes = all_nodes
    g.build(os.path.join(OUT_DIR, "akademie_exterior.glb"))


# ═════════════════════════════════════════════════════════════════════════════
# MODEL 3 — akademie_interior_hall.glb
# ═════════════════════════════════════════════════════════════════════════════

def make_interior_hall():
    g = GLBBuilder()

    stone_floor = g.add_material("Floor_Stone",   0.28, 0.26, 0.24, roughness=0.7)
    stone_wall  = g.add_material("Wall_Stone",    0.22, 0.20, 0.18, roughness=0.88)
    wood_dark   = g.add_material("Wood_Dark",     0.22, 0.14, 0.08, roughness=0.9)
    blue_glow   = g.add_material("Blue_Glow",     0.1, 0.3, 0.8, roughness=0.2, metallic=0.1,
                                   emissive=[0.02, 0.08, 0.35])
    book_mat    = g.add_material("Books",         0.35, 0.12, 0.08, roughness=0.95)
    rune_mat    = g.add_material("Rune_Gold",     0.82, 0.67, 0.12, roughness=0.3, metallic=0.8,
                                   emissive=[0.2, 0.16, 0.0])

    # Floor
    fp, fn, fu, fi = g._plane_geometry(18, 30, subdivisions=4)
    fm = g.add_mesh_from_geo("Floor", fp, fn, fu, fi, stone_floor)
    floor_n = g.add_node("Floor", fm)
    # Floor col
    fc_p, fc_n, fc_u, fc_i = g._plane_geometry(18, 30)
    fc_m = g.add_mesh_from_geo("Floor-col", fc_p, fc_n, fc_u, fc_i, stone_floor)
    floor_col_n = g.add_node("Floor-col", fc_m)

    # Ceiling
    cp, cn, cu, ci = g._plane_geometry(18, 30, cy=9)
    cm = g.add_mesh_from_geo("Ceiling", cp, cn, cu, ci, stone_wall)
    ceil_n = g.add_node("Ceiling", cm)

    # Walls
    wall_nodes = []
    # Left wall
    lp, ln, lu, li = g._box_geometry(0.5, 9, 30, cx=-9, cy=4.5)
    lm = g.add_mesh_from_geo("Wall_Left", lp, ln, lu, li, stone_wall)
    lm_n = g.add_node("Wall_Left", lm)
    lm_c = g.add_mesh_from_geo("Wall_Left-col", lp, ln, lu, li, stone_wall)
    lm_cn = g.add_node("Wall_Left-col", lm_c)
    wall_nodes.extend([lm_n, lm_cn])

    # Right wall
    rp, rn, ru, ri = g._box_geometry(0.5, 9, 30, cx=9, cy=4.5)
    rm = g.add_mesh_from_geo("Wall_Right", rp, rn, ru, ri, stone_wall)
    rm_n = g.add_node("Wall_Right", rm)
    rm_c = g.add_mesh_from_geo("Wall_Right-col", rp, rn, ru, ri, stone_wall)
    rm_cn = g.add_node("Wall_Right-col", rm_c)
    wall_nodes.extend([rm_n, rm_cn])

    # Back wall
    bp, bn, bu, bi = g._box_geometry(18, 9, 0.5, cy=4.5, cz=-15)
    bm = g.add_mesh_from_geo("Wall_Back", bp, bn, bu, bi, stone_wall)
    bm_n = g.add_node("Wall_Back", bm)
    bm_c = g.add_mesh_from_geo("Wall_Back-col", bp, bn, bu, bi, stone_wall)
    bm_cn = g.add_node("Wall_Back-col", bm_c)
    wall_nodes.extend([bm_n, bm_cn])

    # ── Pillars (4 pairs along hall) ──────────────────────────────────────
    pillar_nodes = []
    for px in [-6.5, 6.5]:
        for pz in [-8, -2, 4, 10]:
            pp, pn, pu, pi = g._cylinder_geometry(0.45, 9, 10, cx=px, cy=0, cz=pz)
            pm = g.add_mesh_from_geo(f"Pillar_{px}_{pz}", pp, pn, pu, pi, stone_wall)
            pn_ = g.add_node(f"Pillar_{px}_{pz}", pm)
            # Pillar col
            pc_p, pc_n, pc_u, pc_i = g._cylinder_geometry(0.45, 9, 8, cx=px, cy=0, cz=pz)
            pc_m = g.add_mesh_from_geo(f"Pillar_{px}_{pz}-col", pc_p, pc_n, pc_u, pc_i, stone_wall)
            pc_n_ = g.add_node(f"Pillar_{px}_{pz}-col", pc_m)
            pillar_nodes.extend([pn_, pc_n_])

    # ── Bookshelves along back wall ───────────────────────────────────────
    shelf_nodes = []
    for bsx in [-7, -3.5, 3.5, 7]:
        # Shelf frame
        sfp, sfn, sfu, sfi = g._box_geometry(2.5, 5, 0.4, cx=bsx, cy=2.5, cz=-14.6)
        sfm = g.add_mesh_from_geo(f"ShelfFrame_{bsx}", sfp, sfn, sfu, sfi, wood_dark)
        sfn_ = g.add_node(f"ShelfFrame_{bsx}", sfm)
        # Books on shelves (3 rows)
        for brow in range(3):
            bp2, bn2, bu2, bi2 = g._box_geometry(2.2, 0.4, 0.25,
                                                   cx=bsx, cy=1.0+brow*1.5, cz=-14.7)
            bm2 = g.add_mesh_from_geo(f"Books_{bsx}_{brow}", bp2, bn2, bu2, bi2, book_mat)
            bn2_ = g.add_node(f"Books_{bsx}_{brow}", bm2)
            shelf_nodes.append(bn2_)
        # Shelf collision
        scp, scn, scu, sci = g._box_geometry(2.5, 5, 0.4, cx=bsx, cy=2.5, cz=-14.6)
        scm = g.add_mesh_from_geo(f"ShelfFrame_{bsx}-col", scp, scn, scu, sci, wood_dark)
        scn_ = g.add_node(f"ShelfFrame_{bsx}-col", scm)
        shelf_nodes.extend([sfn_, scn_])

    # ── Blue ambient lighting patches (emissive ceiling squares) ─────────
    light_nodes = []
    for lx, lz in [(0, -6), (-5, 2), (5, 2), (0, 10)]:
        lp, ln, lu, li = g._plane_geometry(1.5, 1.5, cx=lx, cy=8.95, cz=lz)
        lm = g.add_mesh_from_geo(f"Light_{lx}_{lz}", lp, ln, lu, li, blue_glow)
        ln_ = g.add_node(f"Light_{lx}_{lz}", lm)
        light_nodes.append(ln_)

    # ── Rune floor pattern (entrance area) ───────────────────────────────
    rune_nodes = []
    for i in range(6):
        angle = i * np.pi / 3
        rx = 2.5 * np.cos(angle)
        rz = 12 + 2.5 * np.sin(angle)
        rp, rn, ru, ri = g._plane_geometry(0.5, 0.5, cx=rx, cy=0.01, cz=rz)
        rm = g.add_mesh_from_geo(f"FloorRune_{i}", rp, rn, ru, ri, rune_mat)
        rn_ = g.add_node(f"FloorRune_{i}", rm)
        rune_nodes.append(rn_)

    all_nodes = ([floor_n, floor_col_n, ceil_n]
                 + wall_nodes + pillar_nodes + shelf_nodes + light_nodes + rune_nodes)
    g.scene_nodes = all_nodes
    g.build(os.path.join(OUT_DIR, "akademie_interior_hall.glb"))


# ═════════════════════════════════════════════════════════════════════════════
# MODEL 4 — beschwoerungsraum.glb
# ═════════════════════════════════════════════════════════════════════════════

def make_beschwoerungsraum():
    g = GLBBuilder()

    stone_dark  = g.add_material("Stone_Dark",    0.15, 0.13, 0.12, roughness=0.92)
    stone_floor = g.add_material("Floor_Stone",   0.20, 0.18, 0.16, roughness=0.85)
    rune_blue   = g.add_material("Rune_Blue",     0.2, 0.5, 1.0, roughness=0.2, metallic=0.3,
                                   emissive=[0.1, 0.3, 0.9])
    rune_gold   = g.add_material("Rune_Gold",     0.82, 0.67, 0.12, roughness=0.3, metallic=0.8,
                                   emissive=[0.3, 0.22, 0.0])
    particle_mat = g.add_material("Particle_Emit", 0.4, 0.7, 1.0, roughness=0.1, metallic=0.0,
                                   emissive=[0.2, 0.5, 1.0])
    void_mat    = g.add_material("Void_Dark",     0.05, 0.05, 0.08, roughness=0.95)

    # Circular floor (approximated with cylinder disc)
    fp, fn, fu, fi = g._cylinder_geometry(9, 0.15, segments=16, cy=-0.15)
    fm = g.add_mesh_from_geo("Floor", fp, fn, fu, fi, stone_floor)
    floor_n = g.add_node("Floor", fm)
    fc_p, fc_n, fc_u, fc_i = g._cylinder_geometry(9, 0.15, segments=12, cy=-0.15)
    fc_m = g.add_mesh_from_geo("Floor-col", fc_p, fc_n, fc_u, fc_i, stone_floor)
    floor_col_n = g.add_node("Floor-col", fc_m)

    # Circular walls
    wp, wn, wu, wi = g._cylinder_geometry(9, 7, segments=20)
    # Invert to make it interior — we'll keep as outer wall
    wm = g.add_mesh_from_geo("Wall_Ring", wp, wn, wu, wi, stone_dark)
    wall_n = g.add_node("Wall_Ring", wm)
    wc_p, wc_n, wc_u, wc_i = g._cylinder_geometry(9, 7, segments=16)
    wc_m = g.add_mesh_from_geo("Wall_Ring-col", wc_p, wc_n, wc_u, wc_i, stone_dark)
    wall_col_n = g.add_node("Wall_Ring-col", wc_m)

    # ── Main rune circle on floor ─────────────────────────────────────────
    rune_ring_nodes = []
    # Outer rune ring (12 rune marks)
    for i in range(12):
        angle = i * np.pi / 6
        rx = 5.5 * np.cos(angle)
        rz = 5.5 * np.sin(angle)
        rp, rn, ru, ri = g._box_geometry(0.4, 0.04, 0.7, cx=rx, cy=0.01, cz=rz)
        rm = g.add_mesh_from_geo(f"RuneMark_{i}", rp, rn, ru, ri, rune_blue)
        rn_ = g.add_node(f"RuneMark_{i}", rm)
        rune_ring_nodes.append(rn_)

    # Inner circle (flat disc glow)
    ip, in_, iu, ii = g._cylinder_geometry(3.5, 0.03, segments=20)
    im = g.add_mesh_from_geo("InnerCircle", ip, in_, iu, ii, rune_blue)
    inner_n = g.add_node("InnerCircle", im)

    # Center focus point
    cfp, cfn, cfu, cfi = g._cylinder_geometry(0.5, 0.04, segments=12)
    cfm = g.add_mesh_from_geo("CenterFocus", cfp, cfn, cfu, cfi, rune_gold)
    cf_n = g.add_node("CenterFocus", cfm)

    # Star lines (6 lines from center to outer ring)
    line_nodes = []
    for i in range(6):
        angle = i * np.pi / 3
        rx = 2.75 * np.cos(angle)
        rz = 2.75 * np.sin(angle)
        # Line as thin flat box
        line_angle = np.degrees(angle)
        lp, ln, lu, li = g._box_geometry(5.5, 0.03, 0.08, cx=rx, cy=0.01, cz=rz)
        lm = g.add_mesh_from_geo(f"RuneLine_{i}", lp, ln, lu, li, rune_blue)
        ln_ = g.add_node(f"RuneLine_{i}", lm)
        line_nodes.append(ln_)

    # ── Particle emitter markers (5 small glowing spheres/pillars) ────────
    emitter_nodes = []
    for i in range(5):
        angle = i * 2 * np.pi / 5
        ex = 4.0 * np.cos(angle)
        ez = 4.0 * np.sin(angle)
        ep, en, eu, ei = g._cylinder_geometry(0.12, 0.6, 8, cx=ex, cy=0, cz=ez)
        em = g.add_mesh_from_geo(f"Emitter_{i}", ep, en, eu, ei, particle_mat)
        en_ = g.add_node(f"Emitter_{i}", em)
        emitter_nodes.append(en_)

    # ── Stone pillars in room corners ─────────────────────────────────────
    pillar_nodes = []
    for i in range(4):
        angle = i * np.pi / 2 + np.pi / 4
        px = 7.5 * np.cos(angle)
        pz = 7.5 * np.sin(angle)
        pp, pn, pu, pi = g._cylinder_geometry(0.5, 7.5, 8, cx=px, cy=0, cz=pz)
        pm = g.add_mesh_from_geo(f"Pillar_{i}", pp, pn, pu, pi, stone_dark)
        pn_ = g.add_node(f"Pillar_{i}", pm)
        pc_p, pc_n, pc_u, pc_i = g._cylinder_geometry(0.5, 7.5, 6, cx=px, cy=0, cz=pz)
        pc_m = g.add_mesh_from_geo(f"Pillar_{i}-col", pc_p, pc_n, pc_u, pc_i, stone_dark)
        pc_n_ = g.add_node(f"Pillar_{i}-col", pc_m)
        pillar_nodes.extend([pn_, pc_n_])

    # ── Ceiling with void center ──────────────────────────────────────────
    # Outer ceiling ring
    cp, cn, cu, ci = g._cylinder_geometry(9, 0.3, segments=16, cy=7)
    cm = g.add_mesh_from_geo("Ceiling_Ring", cp, cn, cu, ci, stone_dark)
    ceil_n = g.add_node("Ceiling_Ring", cm)

    # Void center (dark disc)
    vp, vn, vu, vi = g._cylinder_geometry(4, 0.1, segments=16, cy=7.1)
    vm = g.add_mesh_from_geo("Void_Center", vp, vn, vu, vi, void_mat)
    void_n = g.add_node("Void_Center", vm)

    all_nodes = ([floor_n, floor_col_n, wall_n, wall_col_n, inner_n, cf_n, ceil_n, void_n]
                 + rune_ring_nodes + line_nodes + emitter_nodes + pillar_nodes)
    g.scene_nodes = all_nodes
    g.build(os.path.join(OUT_DIR, "beschwoerungsraum.glb"))


# ═════════════════════════════════════════════════════════════════════════════
# MODEL 5 — aldenmere_props.glb
# ═════════════════════════════════════════════════════════════════════════════

def make_props():
    g = GLBBuilder()

    stone_mat  = g.add_material("Stone",       0.35, 0.32, 0.28, roughness=0.88)
    wood_mat   = g.add_material("Wood",        0.42, 0.28, 0.14, roughness=0.9)
    iron_mat   = g.add_material("Iron",        0.15, 0.15, 0.15, roughness=0.6, metallic=0.5)
    glow_mat   = g.add_material("LanternGlow", 1.0, 0.85, 0.4, roughness=0.9,
                                 emissive=[1.0, 0.85, 0.4])
    cloth_mat  = g.add_material("Cloth_Flag",  0.55, 0.10, 0.10, roughness=0.95)
    gold_mat   = g.add_material("Gold_Emblem", 0.82, 0.67, 0.12, roughness=0.3, metallic=0.8)
    water_mat  = g.add_material("Water",       0.1, 0.25, 0.45, roughness=0.2, metallic=0.1)

    all_nodes = []

    # ── PROP 1: Fountain (standalone) — offset at (0,0,0) ─────────────────
    # Basin
    bp, bn, bu, bi = g._cylinder_geometry(1.8, 0.45, 14)
    bm = g.add_mesh_from_geo("Fountain_Basin", bp, bn, bu, bi, stone_mat)
    bn_ = g.add_node("Fountain_Basin", bm, translation=[0, 0, 0])
    bc_p, bc_n, bc_u, bc_i = g._cylinder_geometry(1.8, 0.45, 10)
    bc_m = g.add_mesh_from_geo("Fountain_Basin-col", bc_p, bc_n, bc_u, bc_i, stone_mat)
    bc_n_ = g.add_node("Fountain_Basin-col", bc_m, translation=[0, 0, 0])
    # Pillar
    pp, pn, pu, pi = g._cylinder_geometry(0.14, 1.5, cx=0, cy=0.45, cz=0)
    pm = g.add_mesh_from_geo("Fountain_Pillar", pp, pn, pu, pi, stone_mat)
    pn_ = g.add_node("Fountain_Pillar", pm)
    # Water
    wp, wn, wu, wi = g._plane_geometry(3.0, 3.0, cy=0.43)
    wm = g.add_mesh_from_geo("Fountain_Water", wp, wn, wu, wi, water_mat)
    wn_ = g.add_node("Fountain_Water", wm)
    all_nodes.extend([bn_, bc_n_, pn_, wn_])

    # ── PROP 2: Market Stand — offset at (8,0,0) ──────────────────────────
    ox = 8.0
    # Table top
    tp, tn, tu, ti = g._box_geometry(2.5, 0.08, 1.2, cx=ox, cy=0.8)
    tm = g.add_mesh_from_geo("MarketTable", tp, tn, tu, ti, wood_mat)
    tm_n = g.add_node("MarketTable", tm)
    tc_m = g.add_mesh_from_geo("MarketTable-col", tp, tn, tu, ti, wood_mat)
    tc_n = g.add_node("MarketTable-col", tc_m)
    # Legs
    for lx, lz in [(-1.1, -0.5), (1.1, -0.5), (-1.1, 0.5), (1.1, 0.5)]:
        lp, ln, lu, li = g._box_geometry(0.08, 0.8, 0.08, cx=ox+lx, cy=0.4, cz=lz)
        lm = g.add_mesh_from_geo(f"StandLeg_{ox}{lx}{lz}", lp, ln, lu, li, wood_mat)
        ln_ = g.add_node(f"StandLeg_{ox}{lx}{lz}", lm)
        all_nodes.append(ln_)
    # Canopy frame
    cap, can, cau, cai = g._box_geometry(3.0, 0.06, 1.6, cx=ox, cy=2.2)
    cam = g.add_mesh_from_geo("Canopy", cap, can, cau, cai, cloth_mat)
    can_ = g.add_node("Canopy", cam)
    # Canopy poles
    for px in [-1.4, 1.4]:
        pp2, pn2, pu2, pi2 = g._cylinder_geometry(0.05, 2.2, 6, cx=ox+px, cy=0)
        pm2 = g.add_mesh_from_geo(f"Canopy_pole_{px}", pp2, pn2, pu2, pi2, wood_mat)
        pn2_ = g.add_node(f"Canopy_pole_{px}", pm2)
        all_nodes.append(pn2_)
    all_nodes.extend([tm_n, tc_n, can_])

    # ── PROP 3: Street Lantern — offset at (16,0,0) ───────────────────────
    ox2 = 16.0
    # Post
    lp2, ln2, lu2, li2 = g._cylinder_geometry(0.07, 3.5, 8, cx=ox2, cy=0)
    lm2 = g.add_mesh_from_geo("LanternPost", lp2, ln2, lu2, li2, iron_mat)
    lm2_n = g.add_node("LanternPost", lm2)
    lp2c, ln2c, lu2c, li2c = g._cylinder_geometry(0.07, 3.5, 6, cx=ox2, cy=0)
    lmc2 = g.add_mesh_from_geo("LanternPost-col", lp2c, ln2c, lu2c, li2c, iron_mat)
    lmc2_n = g.add_node("LanternPost-col", lmc2)
    # Cap arm
    ap2, an2, au2, ai2 = g._box_geometry(0.5, 0.06, 0.06, cx=ox2, cy=3.5)
    am2 = g.add_mesh_from_geo("LanternArm", ap2, an2, au2, ai2, iron_mat)
    am2_n = g.add_node("LanternArm", am2)
    # Housing
    lbp, lbn, lbu, lbi = g._box_geometry(0.3, 0.4, 0.3, cx=ox2+0.25, cy=3.45)
    lbm = g.add_mesh_from_geo("LanternHousing", lbp, lbn, lbu, lbi, iron_mat)
    lbm_n = g.add_node("LanternHousing", lbm)
    # Glow
    glp, gln, glu, gli = g._box_geometry(0.18, 0.25, 0.18, cx=ox2+0.25, cy=3.5)
    glm = g.add_mesh_from_geo("LanternGlow", glp, gln, glu, gli, glow_mat)
    glm_n = g.add_node("LanternGlow", glm)
    all_nodes.extend([lm2_n, lmc2_n, am2_n, lbm_n, glm_n])

    # ── PROP 4: Valdris Banner/Flag — offset at (24,0,0) ──────────────────
    ox3 = 24.0
    # Flag pole
    pp3, pn3, pu3, pi3 = g._cylinder_geometry(0.06, 6, 8, cx=ox3, cy=0)
    pm3 = g.add_mesh_from_geo("FlagPole", pp3, pn3, pu3, pi3, iron_mat)
    pm3_n = g.add_node("FlagPole", pm3)
    pc3 = g.add_mesh_from_geo("FlagPole-col", pp3, pn3, pu3, pi3, iron_mat)
    pc3_n = g.add_node("FlagPole-col", pc3)
    # Flag cloth
    fp3, fn3, fu3, fi3 = g._box_geometry(1.5, 2.0, 0.04, cx=ox3+0.75, cy=4.8)
    fm3 = g.add_mesh_from_geo("Flag_Cloth", fp3, fn3, fu3, fi3, cloth_mat)
    fm3_n = g.add_node("Flag_Cloth", fm3)
    # Gold emblem (hexagon approximation)
    ep3, en3, eu3, ei3 = g._cylinder_geometry(0.35, 0.04, 6, cx=ox3+0.75, cy=4.8, cz=0.025)
    em3 = g.add_mesh_from_geo("Emblem_Valdris", ep3, en3, eu3, ei3, gold_mat)
    em3_n = g.add_node("Emblem_Valdris", em3)
    all_nodes.extend([pm3_n, pc3_n, fm3_n, em3_n])

    # ── PROP 5: Wooden Barrel — offset at (32,0,0) ────────────────────────
    ox4 = 32.0
    # Barrel body (cylinder)
    bp4, bn4, bu4, bi4 = g._cylinder_geometry(0.45, 0.9, 12, cx=ox4, cy=0)
    bm4 = g.add_mesh_from_geo("Barrel_Body", bp4, bn4, bu4, bi4, wood_mat)
    bm4_n = g.add_node("Barrel_Body", bm4)
    bc4 = g.add_mesh_from_geo("Barrel_Body-col", bp4, bn4, bu4, bi4, wood_mat)
    bc4_n = g.add_node("Barrel_Body-col", bc4)
    # Metal rings (3)
    for ring_y in [0.15, 0.45, 0.75]:
        rp4, rn4, ru4, ri4 = g._cylinder_geometry(0.47, 0.06, 12, cx=ox4, cy=ring_y)
        rm4 = g.add_mesh_from_geo(f"BarrelRing_{ring_y}", rp4, rn4, ru4, ri4, iron_mat)
        rm4_n = g.add_node(f"BarrelRing_{ring_y}", rm4)
        all_nodes.append(rm4_n)
    all_nodes.extend([bm4_n, bc4_n])

    g.scene_nodes = all_nodes
    g.build(os.path.join(OUT_DIR, "aldenmere_props.glb"))


# ═════════════════════════════════════════════════════════════════════════════
# Main
# ═════════════════════════════════════════════════════════════════════════════

if __name__ == "__main__":
    print("DataSoul — Aldenmere Assets Generator")
    print("=" * 45)
    print("\nGenerating 3D models...")
    make_main_square()
    make_akademie_exterior()
    make_interior_hall()
    make_beschwoerungsraum()
    make_props()
    print("\nAll GLB models generated successfully!")
