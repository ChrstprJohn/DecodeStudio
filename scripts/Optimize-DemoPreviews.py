from pathlib import Path
from PIL import Image, ImageDraw
import json

root = Path(__file__).resolve().parents[1]
source = root / '.taste-skill/demo-captures'
output = root / 'MyPortfolio/wwwroot/images/projects'
output.mkdir(parents=True, exist_ok=True)
audit = json.loads((source/'audit.json').read_text())
sheet = Image.new('RGB', (1200, 1320), '#eeeeee')
draw = ImageDraw.Draw(sheet)
for index, item in enumerate(audit):
    assert item['sectionCount'] == 1 and item['footerCount'] == 0, item['key'] + ': sample must be hero-only'
    assert not item['overflow'], item['key'] + ': horizontal overflow'
    assert all(image['loaded'] for image in item['images']), item['key'] + ': image did not load'
    assert all(link['exists'] for link in item['navLinks']), item['key'] + ': broken navigation anchor'
    image = Image.open(source/(item['key']+'.raw')).convert('RGB')
    assert image.width >= 1500 and image.height == 800, (item['key'], image.size)
    image.save(output/(item['key']+'.webp'), 'WEBP', quality=90, method=6)
    image.thumbnail((580, 292))
    x=10+(index%2)*600;y=10+(index//2)*330
    sheet.paste(image,(x,y+24))
    draw.text((x,y), item['slug'].upper(), fill='#222222')
sheet.save(root/'.taste-skill/eight-website-concepts.jpg', quality=93)
print('Saved eight optimized WebP hero captures and a review sheet. All images and navigation anchors passed.')

