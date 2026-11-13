"""Validate the authored animation pipeline without Unity or player save access."""
from pathlib import Path
import hashlib
import json
import math
import re
from BuffmageAtlas import inspect, read_png

ROOT = Path(__file__).resolve().parents[1]
OUT = ROOT / "Assets/Resources/Heroes/Buffmage/Animations"
checks = 0
def check(ok, message):
    global checks
    checks += 1
    assert ok, message
def guid(path):
    return re.search(r"^guid: (\w+)", Path(str(path)+".meta").read_text(), re.M)[1]

manifest = json.loads((OUT / "BuffmageAtlas.json").read_text())
sheet = OUT / manifest.get("sheet", "BuffmageSheet-v1.png")
sheet_guid = guid(sheet)
report = inspect(sheet)
if "sheets" in manifest:
    all_cells = []
    for source in manifest["sheets"]:
        path = OUT / source["sheet"]
        check(hashlib.sha256(path.read_bytes()).hexdigest() == source["sha256"], "Source changed: " + path.name)
        all_cells.extend(inspect(path)["cells"])
    report["cells"] = all_cells
if "bookGripPixels" in manifest:
    book_path = OUT / manifest["frames"][8].get("sheet",sheet.name)
    _, image_height, pixels = read_png(book_path)
    for i,(x,y) in enumerate(manifest["bookGripPixels"]):
        f = manifest["frames"][8+i]
        alpha = pixels[image_height-f["y"]-f["height"]+y][(f["x"]+x)*4+3]
        check(alpha >= 192, "Book grip must lie on the hand artwork, frame " + str(i))
if "sourceFrames" in manifest and "sheets" not in manifest:
    report["cells"] = [report["cells"][i] for i in manifest["sourceFrames"]]
check(hashlib.sha256(sheet.read_bytes()).hexdigest() == manifest["sourceSha256"], "Source raster changed after authoring.")
check((manifest["columns"], manifest["rows"]) == (report["columns"], report["rows"]), "Atlas grid mismatch.")
check(len(manifest["frames"]) == 48 and report["cellWidth"] == report["cellHeight"], "48 equal-size cells required.")
check(manifest["facing"] == "right", "Battle art must be authored facing the enemy on the right.")
idle_height = sorted(c["bounds"][3]-c["bounds"][1]+1 for c in report["cells"][:8])
median_height = (idle_height[3]+idle_height[4])*.5 / manifest["pixelsPerUnit"]
check(math.isclose(median_height / manifest["knightReferenceHeight"], 1.05, abs_tol=.001), "Buffmage stature must match the Knight reference, with 5% extra bulk.")
meta = Path(str(sheet)+".meta").read_text()
check("spriteMode: 2" in meta and "filterMode: 0" in meta and "enableMipMap: 0" in meta, "Sprite import settings.")
check("textureCompression: 1" not in meta, "Atlas must stay uncompressed.")
all_ids = set()
for frame, cell in zip(manifest["frames"], report["cells"]):
    i = len(all_ids)
    check(frame["id"] not in all_ids, "Sprite IDs must be unique.")
    all_ids.add(frame["id"])
    frame_meta = Path(str(OUT / frame.get("sheet",sheet.name))+".meta").read_text()
    check(f"internalID: {frame['id']}" in frame_meta and f"name: {frame['name']}" in frame_meta, "Sprite metadata binding.")
    x0,y0,x1,y1 = cell["bounds"]
    check(x0 > 0 and y0 > 0 and x1 < frame["width"]-1 and y1 < frame["height"]-1, "Opaque sprite touches cell border: " + frame["name"])
    check(cell["red"] == 0 and cell["pixels"] > frame["width"]*frame["height"]*.02, "Missing body or matte survived pixel alpha cutoff.")
    if i not in (25,26,27,28):
        foot = frame["height"] - 1 - y1
        check(abs(frame["pivot"][1]*frame["height"] - foot) < .001, "Ground pivot moved: " + frame["name"])
    check(0 <= frame["pivot"][1] < 1, "Pivot outside cell.")

used = set()
for name, clip in manifest["clips"].items():
    path = OUT / (name + ".anim")
    text = path.read_text()
    check(path.exists() and Path(str(path)+".meta").exists(), "Missing clip or GUID: " + name)
    check("attribute: m_Sprite" in text and "classID: 212" in text and "m_PositionCurves: []" in text, "Clip must drive root SpriteRenderer only.")
    check("m_Events: []" in text, "Damage cannot depend on animation events.")
    check(f"m_LoopTime: {int(clip['loop'])}" in text, "Wrong loop flag: " + name)
    stop = float(re.search(r"m_StopTime: ([\d.]+)", text)[1])
    check(math.isclose(stop, clip["duration"], abs_tol=1e-7), "Clip duration mismatch.")
    keys = re.findall(r"- time: ([\d.]+)\n      value: \{fileID: (\d+), guid: (\w+)", text)
    expected = [manifest["frames"][i]["id"] for i in clip["frames"]]
    expected.append(expected[0] if clip["loop"] else expected[-1])
    check([int(k[1]) for k in keys] == expected, "Object-reference curve frame order: " + name)
    expected_guids = [manifest["frames"][i].get("guid",sheet_guid) for i in clip["frames"]]
    expected_guids.append(expected_guids[0] if clip["loop"] else expected_guids[-1])
    check([k[2] for k in keys] == expected_guids, "Clip atlas bindings differ.")
    check(math.isclose(float(keys[-1][0]), stop, abs_tol=1e-7), "Terminal key does not preserve duration.")
    used.update(clip["frames"])
check(used == set(range(48)), "Unused atlas frames.")
idle = manifest["clips"]["Idle"]["frames"]
check(idle[-1] == 1 and idle[0] == 0, "Idle must join with an adjacent pose, without an end-to-start pop.")
check(not manifest["clips"]["Death"]["loop"], "Death cannot loop.")
if "sheets" in manifest:
    _, ih, idle_pixels = read_png(sheet)
    foot_spans = []
    body_heights = []
    for f in manifest["frames"][:8]:
        bottom = ih-f["y"]-f["height"]+f["bounds"][3]
        xs = [x for y in range(bottom-8,bottom+1) for x in range(f["width"])
              if idle_pixels[y][(f["x"]+x)*4+3] >= 192]
        center = (min(xs)+max(xs))*.5
        check(abs(center-f["pivot"][0]*f["width"]) < .001, "Idle foot center drifts.")
        foot_spans.append(max(xs)-min(xs))
        body_heights.append(f["bounds"][3]-f["bounds"][1]+1)
    check(max(foot_spans)-min(foot_spans) <= 2, "Idle feet change width unexpectedly.")
    check(max(body_heights)/min(body_heights) < 1.01, "Idle height changes by more than subtle breathing.")

for marker, clip, expected_frame in [
    ("bookReleaseSeconds", "BookThrow", 13),
    ("leftPunchImpactSeconds", "PunchLeft", 18),
    ("rightPunchImpactSeconds", "PunchRight", 21),
    ("meteorImpactSeconds", "MeteorSlam", 29),
]:
    c = manifest["clips"][clip]
    seconds = manifest["markers"][marker]
    index = round(seconds*c["fps"])
    check(0 < seconds < c["duration"], "Impact time is outside its clip.")
    check(c["frames"][index] == expected_frame, "Impact does not match the contact/release pose.")

controller = (OUT / "Buffmage.controller").read_text()
ids = re.findall(r"^--- !u!\d+ &(\d+)", controller, re.M)
check(len(ids) == len(set(ids)), "Controller local IDs collide.")
for match in re.finditer(r"fileID: (\d+)([^}\n]*)\}", controller):
    if "guid:" not in match[2]:
        check(match[1] == "0" or match[1] in ids, "Dangling controller transition/state reference.")
for name in manifest["clips"]:
    check(f"m_Name: {name}\n" in controller, "Controller state missing: " + name)
    check(guid(OUT / (name+".anim")) in controller, "Controller clip binding missing.")
check("m_ConditionEvent: Dead" in controller and "m_ConditionMode: 2\n    m_ConditionEvent: Dead" in controller, "Hurt/block must not interrupt death.")

prefab = (OUT.parent / "Buffmage.prefab").read_text()
settings = (OUT.parent / "Settings.asset").read_text()
check(guid(OUT / "Buffmage.controller") in prefab, "Prefab uses the old controller.")
check(guid(OUT / "BuffmagePixel.mat") in prefab, "Clean alpha material is not assigned.")
check(guid(OUT / "AnimationSet.asset") in prefab and guid(OUT / "AnimationSet.asset") in settings, "Animation set is not assigned.")
check(str(manifest["frames"][0]["id"]) in prefab and sheet_guid in prefab, "Prefab opening frame is not assigned.")
shader = (OUT / "BuffmagePixelSprite.shader").read_text()
check("clip(source.a - _AlphaCutoff)" in shader and shader.index("clip(source.a") < shader.index("source.a = 1"), "Source matte must be rejected before opacity normalization.")
print(f"PASS: {checks} Buffmage animation atlas, pivots, clip curves, contact timings, controller and prefab checks.")
