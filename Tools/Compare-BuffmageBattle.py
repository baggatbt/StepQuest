"""Offline candidate overlay on a user screenshot; never edits source art.
The original characters remain visible. This is NOT a Unity scene capture.
"""
from pathlib import Path
import ast, json, struct, zlib
from BuffmageAtlas import read_png
ROOT=Path(__file__).resolve().parents[1]
OUT=ROOT/'Assets/Resources/Heroes/Buffmage/Animations'
m=json.loads((OUT/'BuffmageAtlas.json').read_text())
shot=Path('C:/Users/bagga/AppData/Local/Temp/codex-clipboard-3c7620f9-1743-499e-8f83-dfd6035e471c.png')
w,h,rows=read_png(shot)
canvas=bytearray(b''.join(rows))
sw,sh,source=read_png(OUT/m['sheet'])
f=m['frames'][0]
x0,y0,x1,y1=f['bounds']
# Screenshot Knight body is approximately 90 px tall; actual body is 3.75 units.
factor=24/m['pixelsPerUnit']
dw,dh=round((x1-x0+1)*factor),round((y1-y0+1)*factor)
left,bottom=220-dw//2,469
for y in range(dh):
 sy=sh-f['y']-f['height']+y0+min(y1-y0,int(y/factor))
 for x in range(dw):
  sx=f['x']+x0+min(x1-x0,int(x/factor))
  pixel=source[sy][sx*4:sx*4+4]
  if pixel[3]>=192:
   at=((bottom-dh+y)*w+left+x)*4
   canvas[at:at+4]=pixel[:3]+b'\xff'
tree=ast.parse((ROOT/'Tools/Preview-BuffmageAnimations.py').read_text())
font=next(ast.literal_eval(n.value) for n in tree.body if isinstance(n,ast.Assign) and any(isinstance(t,ast.Name) and t.id=='font' for t in n.targets))
font['W']='10001 10001 10001 10101 10101 10101 01010'
for i,c in enumerate('NEW'):
 for y,row in enumerate(font[c].split()):
  for x,bit in enumerate(row):
   if bit=='1':
    at=((474+y)*w+211+i*6+x)*4
    canvas[at:at+4]=bytes([255,233,189,255])
def chunk(k,d): return struct.pack('>I',len(d))+k+d+struct.pack('>I',zlib.crc32(k+d)&0xffffffff)
raw=b''.join(b'\0'+canvas[y*w*4:(y+1)*w*4] for y in range(h))
dest=ROOT/'output/buffmage/battle-art-overlay.png'
dest.write_bytes(b'\x89PNG\r\n\x1a\n'+chunk(b'IHDR',struct.pack('>IIBBBBB',w,h,8,6,0,0,0))+chunk(b'IDAT',zlib.compress(raw,9))+chunk(b'IEND',b''))
print(dest)
