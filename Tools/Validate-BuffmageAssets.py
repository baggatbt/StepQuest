"""Check authored asset bindings without launching Unity or touching player saves."""
from pathlib import Path
import re
import struct

root = Path(__file__).resolve().parents[1]
assets = root / "Assets"
hero = assets / "Resources/Heroes/Buffmage"
checks = 0

def check(value, message):
    global checks
    checks += 1
    assert value, message

by_guid = {}
for meta in assets.rglob("*.meta"):
    text = meta.read_text(encoding="utf-8-sig", errors="replace")
    match = re.search(r"^guid: (\w+)", text, re.M)
    if match:
        by_guid[match[1]] = (Path(str(meta)[:-5]), text)

for path in [*hero.glob("*.asset"), hero / "Buffmage.prefab", hero / "BuffmageFallback.controller"]:
    text = path.read_text()
    for fileid, guid in re.findall(r"fileID: (-?\d+), guid: (\w+)", text):
        if guid.startswith("0000000000000000"):
            continue
        check(guid in by_guid, f"Unresolved GUID in {path.name}: {guid}")
        target, meta = by_guid[guid]
        check(target.exists(), f"Missing referenced asset: {target}")
        if target.suffix == ".png":
            if "spriteMode: 1" in meta:
                check(int(fileid) == 21300000, f"Single-sprite ID is invalid: {target}")
            else:
                check(f"internalID: {fileid}" in meta, f"Sliced frame ID is invalid: {target} {fileid}")

source = (assets / "Scripts/Skills/Skill.cs").read_text().split("public enum SkillType")[1].split("}")[0].split("{")[1]
names = [n.strip() for n in re.sub(r"//[^\n]*", "", source).split(",") if n.strip()]
legacy = ["None", "Slash", "TripleHit", "SwordWave", "GuardSkill", "ShieldSlam", "SlimeCompanionBasicAttack", "WizardBasicAttack", "FirePillar", "Taunt", "ShootArrow", "ArrowRain", "MeleeCombo", "ReflectDamagePassive", "SpeedBreak", "TamedGoblinAttackSkill", "HeavySlash"]
check(names[:len(legacy)] == legacy, "An existing saved skill ID was changed.")
expected = [names.index(n) for n in ("BuffmageFireball", "BuffmageMagicMissile", "BuffmageMeteor")]
data = (hero / "CharacterData.asset").read_text()
for key in ("AvailableSkills", "equippedSkills"):
    packed = bytes.fromhex(re.search(rf"  {key}: (\w+)", data)[1])
    check(list(struct.unpack("<" + "i" * (len(packed) // 4), packed)) == expected, f"Incorrect {key} enum IDs.")
check("  heroID: Buffmage" in data and "  isUnlocked: 0" in data, "Default hero identity or permanent unlock is invalid.")
check("  developmentUnlock: 1" in (hero / "Definition.asset").read_text(), "Default testing availability is disabled.")

prefab = (hero / "Buffmage.prefab").read_text()
ids = re.findall(r"^--- !u!\d+ &(-?\d+)", prefab, re.M)
check(len(ids) == len(set(ids)), "Duplicate local IDs in Buffmage prefab.")
for line in prefab.splitlines():
    if "guid:" in line:
        continue
    for local in re.findall(r"fileID: (-?\d+)", line):
        check(local == "0" or local in ids, f"Dangling prefab reference: {local}")
check("m_TagString: Companion" in prefab and "m_BodyType: 1" in prefab, "Shared battle tag/kinematic body is missing.")
check("characterIDNumber: Buffmage" in prefab, "Hero selection ID collides with Knight.")
for path in (hero / "BuffmageIdle.png", assets / "Ninja Adventure - Asset Pack/Items/Weapons/Book/Sprite.png", assets / "Ninja Adventure - Asset Pack/Ui/Skill Icon/Spell/BookFire.png", assets / "Ninja Adventure - Asset Pack/Ui/Skill Icon/Job & Action/Punch.png", assets / "Ninja Adventure - Asset Pack/Ui/Skill Icon/Spell/RockSpike.png", assets / "Ninja Adventure - Asset Pack/FX/Magic/Spark/SpriteSheet.png", assets / "Resources/Sprites/UI Icons/campfire animation.png"):
    meta = Path(str(path) + ".meta").read_text()
    check("filterMode: 0" in meta and "enableMipMap: 0" in meta, f"Pixel import settings are not crisp: {path}")
png = (hero / "BuffmageIdle.png").read_bytes()
check(png[:8] == b"\x89PNG\r\n\x1a\n" and png[25] == 6, "Character cutout must be a PNG with an alpha channel.")
print(f"PASS: {checks} Buffmage asset, prefab, sprite import, and saved-enum compatibility checks.")
