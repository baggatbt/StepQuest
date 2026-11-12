"""Offline pose preview of authored clip curves (not a Unity render).
Draws the atlas through the same source-alpha cutoff and foot pivots as the
sprite material. The source sheet is read-only.
"""
from pathlib import Path
import json
import math
import struct
import zlib
from BuffmageAtlas import read_png

ROOT = Path(__file__).resolve().parents[1]
OUT = ROOT / "Assets/Resources/Heroes/Buffmage/Animations"
manifest = json.loads((OUT / "BuffmageAtlas.json").read_text())
width, height, source = read_png(OUT / manifest.get("sheet", "BuffmageSheet-v1.png"))
cw = width // 8
ch = height // 6
preview_ppu = 56
render_scale = preview_ppu / manifest["pixelsPerUnit"]
render_width, render_height = round(cw*render_scale), round(ch*render_scale)
tilew, tileh = 280, 480
w, h = tilew*3, tileh*2
panels = ["IDLE", "BOOK", "PUNCH", "METEOR", "HURT", "DEATH"]
font = {
"A":"01110 10001 10001 11111 10001 10001 10001",
"B":"11110 10001 10001 11110 10001 10001 11110",
"C":"01111 10000 10000 10000 10000 10000 01111",
"D":"11110 10001 10001 10001 10001 10001 11110",
"E":"11111 10000 10000 11110 10000 10000 11111",
"H":"10001 10001 10001 11111 10001 10001 10001",
"I":"11111 00100 00100 00100 00100 00100 11111",
"K":"10001 10010 10100 11000 10100 10010 10001",
"L":"10000 10000 10000 10000 10000 10000 11111",
"M":"10001 11011 10101 10101 10001 10001 10001",
"N":"10001 11001 10101 10011 10001 10001 10001",
"O":"01110 10001 10001 10001 10001 10001 01110",
"P":"11110 10001 10001 11110 10000 10000 10000",
"R":"11110 10001 10001 11110 10100 10010 10001",
"T":"11111 00100 00100 00100 00100 00100 00100",
"U":"10001 10001 10001 10001 10001 10001 01110",
}
background = bytearray(bytes([31,29,28,255])*(w*h))
def rect(canvas,x,y,ww,hh,color):
    pixel = bytes(color)
    for yy in range(max(0,y),min(h,y+hh)):
        left,right = max(0,x),min(w,x+ww)
        canvas[(yy*w+left)*4:(yy*w+right)*4] = pixel*(right-left)
for i,label in enumerate(panels):
    x,y = i%3*tilew,i//3*tileh
    rect(background,x+3,y+3,tilew-6,tileh-6,[43,36,33,255])
    for xx,yy,ww,hh in [(x+3,y+3,tilew-6,2),(x+3,y+tileh-5,tilew-6,2),(x+3,y+3,2,tileh-6),(x+tilew-5,y+3,2,tileh-6)]:
        rect(background,xx,yy,ww,hh,[111,87,60,255])
    for n,char in enumerate(label):
        for fy,row in enumerate(font[char].split()):
            for fx,bit in enumerate(row):
                if bit=="1":
                    rect(background,x+(tilew-len(label)*12)//2+n*12+fx*2,y+17+fy*2,2,2,[225,204,161,255])
    rect(background,x+25,y+459,tilew-50,1,[78,65,47,255])

# Precompute opaque row runs after the material's 0.75 alpha cutoff.
runs = []
for i,f in enumerate(manifest["frames"]):
    row,col = divmod(i,8)
    frame_runs = []
    for y in range(render_height):
        line = source[row*ch+min(ch-1,int(y/render_scale))]; x=0
        while x<render_width:
            sx=min(cw-1,int(x/render_scale))
            if line[(col*cw+sx)*4+3]<192:
                x+=1; continue
            start=x; pixels=bytearray()
            while x<render_width:
                sx=min(cw-1,int(x/render_scale))
                k=(col*cw+sx)*4
                if line[k+3]<192: break
                pixels.extend(line[k:k+3]); pixels.append(255); x+=1
            frame_runs.append((y,start,bytes(pixels)))
    runs.append(frame_runs)

def sample_sequence(sequence,t):
    total=sum(d for name,d in sequence)
    t=t%total
    phase=t
    for name,duration in sequence:
        if t<duration:
            clip=manifest["clips"][name]
            local=t%clip["duration"] if clip["loop"] else min(t,clip["duration"]-1e-6)
            index=min(len(clip["frames"])-1,int(local*clip["fps"]))
            return clip["frames"][index],phase
        t-=duration
    raise AssertionError("Sequence sampling failed")

idle=manifest["clips"]["Idle"]["duration"]
sequences=[
    [("Idle",idle)],
    [("BookWindup",.4),("BookHold",.65),("BookThrow",5/12),("Idle",.5)],
    [("PunchLeft",4/24),("PunchRight",4/24),("Idle",.2)],
    [("MeteorReady",.4),("MeteorLaunch",.2),("MeteorAir",.55-.2-1/12),("MeteorSlam",.25),("MeteorRecover",.2),("Idle",.5)],
    [("Hurt",8/18),("Idle",.8)],
    [("Death",.8),("Death",.65),("Idle",.5)],
]
def render(t):
    canvas=bytearray(background)
    for panel,sequence in enumerate(sequences):
        index,phase=sample_sequence(sequence,t)
        # Hold the corpse on its final pose instead of restarting it.
        if panel==5 and .8<=phase<1.45: index=47
        f=manifest["frames"][index]
        lift=0
        if panel==3 and .4<=phase<=.95:
            u=(phase-.4)/.55
            lift=round(4*u*(1-u)*2.5*preview_ppu)
        ox=panel%3*tilew+tilew//2-round(cw*.5*render_scale)
        oy=panel//3*tileh+458-round((ch-f["pivot"][1]*ch)*render_scale)-lift
        for sy,sx,pixels in runs[index]:
            yy=oy+sy;xx=ox+sx
            if 0<=yy<h and 0<=xx and xx+len(pixels)//4<=w:
                offset=(yy*w+xx)*4
                canvas[offset:offset+len(pixels)]=pixels
    return canvas

def compress(canvas):
    stride=w*4
    return zlib.compress(b"".join(b"\x00"+canvas[y*stride:(y+1)*stride] for y in range(h)),6)

def chunk(name,data):
    return struct.pack(">I",len(data))+name+data+struct.pack(">I",zlib.crc32(name+data)&0xffffffff)
header=b"\x89PNG\r\n\x1a\n"+chunk(b"IHDR",struct.pack(">IIBBBBB",w,h,8,6,0,0,0))
output=ROOT/"output/buffmage"
output.mkdir(parents=True,exist_ok=True)
count,fps=120,24
animation=bytearray(header+chunk(b"acTL",struct.pack(">II",count,0)))
serial=0
previous=None
for frame in range(count):
    canvas=render(frame/fps)
    if previous is None:
        encoded=canvas
    else:
        # Transparent unchanged pixels let APNG reuse earlier frame content.
        encoded=bytearray(len(canvas))
        current_words=memoryview(canvas).cast("I")
        old_words=memoryview(previous).cast("I")
        output_words=memoryview(encoded).cast("I")
        for index in range(len(current_words)):
            if current_words[index]!=old_words[index]: output_words[index]=current_words[index]
    compressed=compress(encoded)
    animation.extend(chunk(b"fcTL",struct.pack(">IIIIIHHBB",serial,w,h,0,0,1,fps,0,0 if frame==0 else 1)));serial+=1
    if frame==0:
        animation.extend(chunk(b"IDAT",compressed))
        (output/"animation-contact.png").write_bytes(header+chunk(b"IDAT",compressed)+chunk(b"IEND",b""))
    else:
        animation.extend(chunk(b"fdAT",struct.pack(">I",serial)+compressed));serial+=1
    previous=canvas
animation.extend(chunk(b"IEND",b""))
(output/"animation-preview.png").write_bytes(animation)
for seconds,name in [(1.1,"animation-contact-action.png"),(1.5,"animation-contact-corpse.png")]:
    (output/name).write_bytes(header+chunk(b"IDAT",compress(render(seconds)))+chunk(b"IEND",b""))
print("Wrote a 120-frame animated PNG and pose contact previews to output/buffmage.")
