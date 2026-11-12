"""Author Unity slicing, clips and controller for the generated Buffmage sheet.
This writes Unity-native assets and metadata; it does not edit the raster art.
"""
from pathlib import Path
import hashlib
import json
import re
import uuid
import statistics
from BuffmageAtlas import inspect, read_png

ROOT = Path(__file__).resolve().parents[1]
HERO = ROOT / "Assets/Resources/Heroes/Buffmage"
OUT = HERO / "Animations"
SHEET = OUT / "BuffmageSheet-v2.png"
KNIGHT_HEIGHT_MULTIPLIER = 1.05
CLIPS = {
    "Idle":       (list(range(0, 8)) + list(range(6, 0, -1)), 8, True),
    "BookWindup": (list(range(8, 12)), 10, False),
    "BookHold":   ([11], 4, True),
    "BookThrow":  ([11, 12, 13, 14, 15], 12, False),
    "PunchLeft":  ([16, 17, 18, 19], 24, False),
    "PunchRight": ([20, 21, 22, 23], 24, False),
    "MeteorReady": ([24], 4, True),
    "MeteorLaunch": ([24, 25], 10, False),
    "MeteorAir": ([26, 27], 8, True),
    "MeteorSlam": ([28, 29, 30], 12, False),
    "MeteorRecover": ([30, 31], 10, False),
    "Hurt": (list(range(32, 40)), 18, False),
    "Death": (list(range(40, 48)), 10, False),
    "Block": ([32, 33, 32], 20, False),
}
AUTO = dict(BookWindup="BookHold", BookThrow="Idle", PunchLeft="Idle", PunchRight="Idle",
            MeteorLaunch="MeteorAir", MeteorSlam="MeteorRecover", MeteorRecover="Idle",
            Hurt="Idle", Block="Idle")

def metadata(path, extra=""):
    target = Path(str(path) + ".meta")
    if not target.exists():
        target.write_text("fileFormatVersion: 2\nguid: " + uuid.uuid4().hex + "\n" + extra)
    return re.search(r"^guid: (\w+)", target.read_text(), re.M)[1]

def native(path, fileid):
    return metadata(path, f"NativeFormatImporter:\n  externalObjects: {{}}\n  mainObjectFileID: {fileid}\n")

def yaml_start():
    return "%YAML 1.1\n%TAG !u! tag:unity3d.com,2011:\n"

def object_start(kind, fileid, typename):
    return f"""--- !u!{kind} &{fileid}
{typename}:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
"""

def scriptable(script_guid, name):
    return yaml_start() + object_start(114, 11400000, "MonoBehaviour") + f"""  m_GameObject: {{fileID: 0}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {script_guid}, type: 3}}
  m_Name: {name}
  m_EditorClassIdentifier:
"""

OUT.mkdir(parents=True, exist_ok=True)
metadata(OUT, "folderAsset: yes\nDefaultImporter:\n  externalObjects: {}\n")
set_script = ROOT / "Assets/Scripts/VisualEffects/BuffmageAnimationSet.cs"
driver_script = ROOT / "Assets/Scripts/VisualEffects/BuffmageAnimationDriver.cs"
set_guid = metadata(set_script, "MonoImporter:\n  externalObjects: {}\n")
driver_guid = metadata(driver_script, "MonoImporter:\n  externalObjects: {}\n")
sheet_guid = metadata(SHEET)
report = inspect(SHEET)
cw, ch = report["cellWidth"], report["cellHeight"]
assert cw == ch and sum(c["red"] for c in report["cells"]) == 0
# Compare opaque bodies, not the transparent dimensions of the two atlases.
knight_path = ROOT / "Assets/Knight/Sprites/HeroKnight.png"
kw, kh, knight_rows = read_png(knight_path)
knight_meta = Path(str(knight_path)+".meta").read_text()
entry = knight_meta.split("name: HeroKnight_0")[1].split("alignment:")[0]
kx, ky, kcw, kch = [int(re.search(r"\b"+key+r": (\d+)", entry)[1]) for key in ("x","y","width","height")]
opaque_y = [y for y in range(kch) for x in range(kcw) if knight_rows[kh-ky-kch+y][(kx+x)*4+3] >= 192]
knight_ppu = float(re.search(r"spritePixelsToUnits: ([\d.]+)", knight_meta)[1])
knight_prefab = (ROOT / "Assets/Resources/PreFab/PlayableCharacters/Knight.prefab").read_text()
knight_scale = float(re.search(r"m_LocalScale: \{x: [^,]+, y: ([\d.]+)", knight_prefab)[1])
knight_collider = knight_prefab.split("CapsuleCollider2D:")[1].split("--- !u!")[0]
kcw_world, kch_world = [float(v)*knight_scale for v in re.search(r"m_Size: \{x: ([\d.]+), y: ([\d.]+)\}", knight_collider).groups()]
offset_y = float(re.search(r"m_Offset: \{x: [^,]+, y: ([\d.]+)\}", knight_collider)[1])*knight_scale
knight_height = (max(opaque_y)-min(opaque_y)+1) / knight_ppu * knight_scale
world_height = knight_height * KNIGHT_HEIGHT_MULTIPLIER
idle_body_pixels = statistics.median(c["bounds"][3]-c["bounds"][1]+1 for c in report["cells"][:8])
pixels_per_unit = idle_body_pixels / world_height
punch_reach = statistics.mean(report["cells"][i]["bounds"][2] - cw*.5 for i in (18,21)) / pixels_per_unit
frame_ids = [21310000 + i for i in range(48)]
rows = ["Idle", "Book", "Punch", "Meteor", "Hurt", "Death"]
frames = []
idle_ground = statistics.median(ch - 1 - c["bounds"][3] for c in report["cells"][:8])
for i, cell in enumerate(report["cells"]):
    row, column = divmod(i, 8)
    # Feet stay on the ground in standing, hurt, crouch and corpse frames.
    # Airborne poses retain their internal lift as the root follows its arc.
    ground = idle_ground if row == 3 and column in (1, 2, 3, 4) else ch - 1 - cell["bounds"][3]
    frames.append(dict(name=f"Buffmage_{rows[row]}_{column:02}", id=frame_ids[i],
        x=column*cw, y=(5-row)*ch, width=cw, height=ch, pivot=[.5, ground/ch], bounds=cell["bounds"]))

source_meta = Path(str(HERO / "BuffmageIdle.png") + ".meta").read_text()
source_meta = re.sub(r"^guid: \w+", "guid: " + sheet_guid, source_meta, count=1, flags=re.M)
name_table = "  internalIDToNameTable:\n" + "".join(
    f"  - first:\n      213: {f['id']}\n    second: {f['name']}\n" for f in frames)
source_meta = source_meta.replace("  internalIDToNameTable: []\n", name_table)
source_meta = source_meta.replace("  spriteMode: 1", "  spriteMode: 2")
source_meta = re.sub(r"spritePixelsToUnits: [^\n]+", f"spritePixelsToUnits: {pixels_per_unit:.9g}", source_meta)
source_meta = source_meta.replace("filterMode: 1", "filterMode: 0").replace("textureCompression: 1", "textureCompression: 0")
sprite_sheet = "  spriteSheet:\n    serializedVersion: 2\n    sprites:\n"
for f in frames:
    sprite_guid = uuid.uuid5(uuid.UUID(sheet_guid), f["name"]).hex
    sprite_sheet += f"""    - serializedVersion: 2
      name: {f['name']}
      rect:
        serializedVersion: 2
        x: {f['x']}
        y: {f['y']}
        width: {cw}
        height: {ch}
      alignment: 9
      pivot: {{x: 0.5, y: {f['pivot'][1]:.9g}}}
      border: {{x: 0, y: 0, z: 0, w: 0}}
      outline: []
      physicsShape: []
      tessellationDetail: 0
      bones: []
      spriteID: {sprite_guid}
      internalID: {f['id']}
      vertices: []
      indices:
      edges: []
      weights: []
"""
sprite_sheet += """    outline: []
    physicsShape: []
    bones: []
    spriteID: """ + uuid.uuid5(uuid.UUID(sheet_guid), "sheet").hex + """
    internalID: 0
    vertices: []
    indices:
    edges: []
    weights: []
    secondaryTextures: []
    nameFileIdTable:
"""
sprite_sheet += "".join(f"      {f['name']}: {f['id']}\n" for f in frames)
source_meta = re.sub(r"  spriteSheet:\n.*?(?=  spritePackingTag:)", sprite_sheet, source_meta, flags=re.S)
Path(str(SHEET) + ".meta").write_text(source_meta)

clip_guids = {}
for name, (indices, fps, loop) in CLIPS.items():
    path = OUT / (name + ".anim")
    clip_guids[name] = native(path, 7400000)
    # A terminal key makes the actual Unity clip length exact and preserves the
    # final corpse pose. Looping clips join back to their opening pose.
    mapping = [frame_ids[i] for i in indices] + [frame_ids[indices[0] if loop else indices[-1]]]
    text = yaml_start() + object_start(74, 7400000, "AnimationClip") + f"""  m_Name: Buffmage_{name}
  serializedVersion: 6
  m_Legacy: 0
  m_Compressed: 0
  m_UseHighQualityCurve: 1
  m_RotationCurves: []
  m_CompressedRotationCurves: []
  m_EulerCurves: []
  m_PositionCurves: []
  m_ScaleCurves: []
  m_FloatCurves: []
  m_PPtrCurves:
  - curve:
"""
    text += "".join(f"    - time: {i/fps:.9g}\n      value: {{fileID: {fileid}, guid: {sheet_guid}, type: 3}}\n" for i, fileid in enumerate(mapping))
    text += f"""    attribute: m_Sprite
    path:
    classID: 212
    script: {{fileID: 0}}
  m_SampleRate: {fps}
  m_WrapMode: 0
  m_Bounds:
    m_Center: {{x: 0, y: 0, z: 0}}
    m_Extent: {{x: 0, y: 0, z: 0}}
  m_ClipBindingConstant:
    genericBindings:
    - serializedVersion: 2
      path: 0
      attribute: 0
      script: {{fileID: 0}}
      typeID: 212
      customType: 23
      isPPtrCurve: 1
    pptrCurveMapping:
"""
    text += "".join(f"    - {{fileID: {fileid}, guid: {sheet_guid}, type: 3}}\n" for fileid in mapping)
    text += f"""  m_AnimationClipSettings:
    serializedVersion: 2
    m_AdditiveReferencePoseClip: {{fileID: 0}}
    m_AdditiveReferencePoseTime: 0
    m_StartTime: 0
    m_StopTime: {len(indices)/fps:.9g}
    m_OrientationOffsetY: 0
    m_Level: 0
    m_CycleOffset: 0
    m_HasAdditiveReferencePose: 0
    m_LoopTime: {int(loop)}
    m_LoopBlend: 0
    m_LoopBlendOrientation: 0
    m_LoopBlendPositionY: 0
    m_LoopBlendPositionXZ: 0
    m_KeepOriginalOrientation: 0
    m_KeepOriginalPositionY: 1
    m_KeepOriginalPositionXZ: 0
    m_HeightFromFeet: 0
    m_Mirror: 0
  m_EditorCurves: []
  m_EulerEditorCurves: []
  m_HasGenericRootTransform: 0
  m_HasMotionFloatCurves: 0
  m_Events: []
"""
    path.write_text(text)

controller = OUT / "Buffmage.controller"
controller_guid = native(controller, 9100000)
state_ids = {name: 1102000000 + i for i, name in enumerate(CLIPS)}
transitions = []
auto_ids = {}
for source, target in AUTO.items():
    tid = 1101000000 + len(transitions)
    transitions.append((tid, target, [], True))
    auto_ids[source] = tid
any_ids = []
for target, conditions in [
    ("Death", [("Dead", 1)]),
    ("Hurt", [("IsHurtTrigger", 1), ("Dead", 2)]),
    ("Block", [("BlockTrigger", 1), ("Dead", 2)]),
]:
    tid = 1101000000 + len(transitions)
    transitions.append((tid, target, conditions, False))
    any_ids.append(tid)
text = yaml_start() + object_start(91, 9100000, "AnimatorController") + """  m_Name: Buffmage
  serializedVersion: 5
  m_AnimatorParameters:
"""
for name in ["IsHurtTrigger", "EnemyIsHurtTrigger", "BlockTrigger", "MovementAnimationTrigger", "StopMovementAnimationTrigger", "Attack", "Hit", "Die", "Dead"]:
    text += f"""  - m_Name: {name}
    m_Type: {4 if name == 'Dead' else 9}
    m_DefaultFloat: 0
    m_DefaultInt: 0
    m_DefaultBool: 0
    m_Controller: {{fileID: 9100000}}
"""
text += """  m_AnimatorLayers:
  - serializedVersion: 5
    m_Name: Base Layer
    m_StateMachine: {fileID: 1107000000}
    m_Mask: {fileID: 0}
    m_Motions: []
    m_Behaviours: []
    m_BlendingMode: 0
    m_SyncedLayerIndex: -1
    m_DefaultWeight: 1
    m_IKPass: 0
    m_SyncedLayerAffectsTiming: 0
    m_Controller: {fileID: 9100000}
"""
text += object_start(1107, 1107000000, "AnimatorStateMachine") + """  serializedVersion: 6
  m_Name: Base Layer
  m_ChildStates:
"""
for i, (name, sid) in enumerate(state_ids.items()):
    text += f"  - serializedVersion: 1\n    m_State: {{fileID: {sid}}}\n    m_Position: {{x: {250+(i%4)*220}, y: {80+(i//4)*100}, z: 0}}\n"
text += "  m_ChildStateMachines: []\n  m_AnyStateTransitions:\n" + "".join(f"  - {{fileID: {i}}}\n" for i in any_ids)
text += """  m_EntryTransitions: []
  m_StateMachineTransitions: {}
  m_StateMachineBehaviours: []
  m_DefaultState: {fileID: """ + str(state_ids["Idle"]) + "}\n"
for name, sid in state_ids.items():
    text += object_start(1102, sid, "AnimatorState") + f"""  serializedVersion: 6
  m_Name: {name}
  m_Speed: 1
  m_CycleOffset: 0
"""
    text += f"  m_Transitions:\n  - {{fileID: {auto_ids[name]}}}\n" if name in auto_ids else "  m_Transitions: []\n"
    text += f"""  m_StateMachineBehaviours: []
  m_Position: {{x: 50, y: 50, z: 0}}
  m_IKOnFeet: 0
  m_WriteDefaultValues: 0
  m_Mirror: 0
  m_SpeedParameterActive: 0
  m_MirrorParameterActive: 0
  m_CycleOffsetParameterActive: 0
  m_TimeParameterActive: 0
  m_Motion: {{fileID: 7400000, guid: {clip_guids[name]}, type: 2}}
  m_Tag:
  m_SpeedParameter:
  m_MirrorParameter:
  m_CycleOffsetParameter:
  m_TimeParameter:
"""
for tid, target, conditions, automatic in transitions:
    text += object_start(1101, tid, "AnimatorStateTransition")
    text += "  m_Name:\n  m_Conditions:\n" if conditions else "  m_Name:\n  m_Conditions: []\n"
    for condition, mode in conditions:
        text += f"  - m_ConditionMode: {mode}\n    m_ConditionEvent: {condition}\n    m_EventTreshold: 0\n"
    text += f"""  m_DstStateMachine: {{fileID: 0}}
  m_DstState: {{fileID: {state_ids[target]}}}
  m_Solo: 0
  m_Mute: 0
  m_IsExit: 0
  serializedVersion: 3
  m_TransitionDuration: 0
  m_TransitionOffset: 0
  m_ExitTime: 1
  m_HasExitTime: {int(automatic)}
  m_HasFixedDuration: 1
  m_InterruptionSource: 0
  m_OrderedInterruption: 1
  m_CanTransitionToSelf: 0
"""
controller.write_text(text)

animation_set = OUT / "AnimationSet.asset"
animation_set_guid = native(animation_set, 11400000)
text = scriptable(set_guid, "BuffmageAnimations") + f"  controller: {{fileID: 9100000, guid: {controller_guid}, type: 2}}\n"
text += f"  authoredWorldHeight: {world_height:.9g}\n  punchReach: {punch_reach:.9g}\n"
for name in CLIPS:
    if name != "Block":
        field = name[0].lower() + name[1:]
        text += f"  {field}: {{fileID: 7400000, guid: {clip_guids[name]}, type: 2}}\n"
markers = dict(bookReleaseSeconds=1/12, leftPunchImpactSeconds=2/24, rightPunchImpactSeconds=1/24,
               meteorImpactSeconds=1/12, deathHoldSeconds=.2)
text += "".join(f"  {key}: {value:.9g}\n" for key, value in markers.items())
animation_set.write_text(text)

shader = OUT / "BuffmagePixelSprite.shader"
shader_guid = metadata(shader, "ShaderImporter:\n  externalObjects: {}\n  defaultTextures: []\n  nonModifiableTextures: []\n")
material = OUT / "BuffmagePixel.mat"
material_guid = native(material, 2100000)
material.write_text(yaml_start() + object_start(21, 2100000, "Material") + f"""  serializedVersion: 8
  m_Name: BuffmagePixel
  m_Shader: {{fileID: 4800000, guid: {shader_guid}, type: 3}}
  m_Parent: {{fileID: 0}}
  m_ModifiedSerializedProperties: 0
  m_ValidKeywords:
  - PIXELSNAP_ON
  m_InvalidKeywords: []
  m_LightmapFlags: 4
  m_EnableInstancingVariants: 0
  m_DoubleSidedGI: 0
  m_CustomRenderQueue: -1
  stringTagMap: {{}}
  disabledShaderPasses: []
  m_LockedProperties:
  m_SavedProperties:
    serializedVersion: 3
    m_TexEnvs:
    - _MainTex:
        m_Texture: {{fileID: 2800000, guid: {sheet_guid}, type: 3}}
        m_Scale: {{x: 1, y: 1}}
        m_Offset: {{x: 0, y: 0}}
    m_Ints: []
    m_Floats:
    - _AlphaCutoff: 0.75
    - PixelSnap: 1
    m_Colors:
    - _Color: {{r: 1, g: 1, b: 1, a: 1}}
""")

settings = HERO / "Settings.asset"
text = settings.read_text()
reference = f"  animations: {{fileID: 11400000, guid: {animation_set_guid}, type: 2}}\n"
text = re.sub(r"^  animations:.*\n", "", text, flags=re.M)
text = text.replace("  bookSprite:", reference + "  bookSprite:")
settings.write_text(text)

prefab = HERO / "Buffmage.prefab"
text = prefab.read_text()
text = re.sub(r"m_Controller: \{fileID: 9100000, guid: \w+, type: 2\}", f"m_Controller: {{fileID: 9100000, guid: {controller_guid}, type: 2}}", text, count=1)
text = re.sub(r"m_Sprite: \{fileID: -?\d+, guid: \w+, type: 3\}", f"m_Sprite: {{fileID: {frame_ids[0]}, guid: {sheet_guid}, type: 3}}", text, count=1)
text = re.sub(r"(  m_Materials:\n  - )\{[^\n]+\}", rf"\g<1>{{fileID: 2100000, guid: {material_guid}, type: 2}}", text, count=1)
component_id = 6023901811220
if f"&{component_id}" not in text:
    text = text.replace("  m_Layer: 0\n", f"  - component: {{fileID: {component_id}}}\n  m_Layer: 0\n", 1)
    text += object_start(114, component_id, "MonoBehaviour") + f"""  m_GameObject: {{fileID: 358661848719693019}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {driver_guid}, type: 3}}
  m_Name:
  m_EditorClassIdentifier:
  animations: {{fileID: 11400000, guid: {animation_set_guid}, type: 2}}
"""
prefab.write_text(text)
# Keep the body hitbox in proportion with the corrected visual stature.
start = text.index("CapsuleCollider2D:")
end = text.index("--- !u!", start)
collider = text[start:end]
collider = re.sub(r"m_Offset: \{[^\n]+\}", f"m_Offset: {{x: 0, y: {offset_y*KNIGHT_HEIGHT_MULTIPLIER:.9g}}}", collider, count=1)
collider = re.sub(r"m_Size: \{[^\n]+\}", f"m_Size: {{x: {kcw_world*KNIGHT_HEIGHT_MULTIPLIER:.9g}, y: {kch_world*KNIGHT_HEIGHT_MULTIPLIER:.9g}}}", collider, count=1)
text = text[:start] + collider + text[end:]
prefab.write_text(text)

manifest = dict(sheet=SHEET.name, facing="right", sourceSha256=hashlib.sha256(SHEET.read_bytes()).hexdigest(),
    width=report["width"], height=report["height"], columns=8, rows=6, sourceAlphaCutoff=.75,
    pixelsPerUnit=pixels_per_unit, authoredWorldHeight=world_height, knightReferenceHeight=knight_height,
    knightHeightMultiplier=KNIGHT_HEIGHT_MULTIPLIER, punchReach=punch_reach, frames=frames,
    clips={name: dict(frames=indices, fps=fps, loop=loop, duration=len(indices)/fps)
           for name,(indices,fps,loop) in CLIPS.items()},
    markers=markers, automaticTransitions=AUTO)
manifest_path = OUT / "BuffmageAtlas.json"
metadata(manifest_path, "TextScriptImporter:\n  externalObjects: {}\n")
manifest_path.write_text(json.dumps(manifest, indent=2))
print(f"Authored {len(frames)} sliced sprites, {len(CLIPS)} clips, controller, animation set, material and prefab bindings ({cw}x{ch} cells).")
