const fs = require('fs');
const path = require('path');
const { chromium } = require('playwright');
const sharp = require('sharp');
const root = path.resolve(__dirname, '..');
const qa = path.join(root, 'Art/qa');
fs.mkdirSync(qa, { recursive: true });
const palette = JSON.parse(fs.readFileSync(path.join(__dirname, 'preview-palette.json')));
const about = fs.readFileSync(path.join(root, 'Mod/About/About.xml'), 'utf8');
const version = [...about.match(/<supportedVersions>([\s\S]*?)<\/supportedVersions>/)[1].matchAll(/<li>(.*?)<\/li>/g)].map(x => x[1]).sort((a, b) => a.localeCompare(b, undefined, { numeric: true })).pop();
const source = fs.readFileSync(path.join(root, 'Mod/About/Preview.png')).toString('base64');
const html = `<!doctype html><meta charset="utf-8"><style>
:root{${Object.entries(palette).map(([k, v]) => `--${k}:${v}`).join(';')}}
*{box-sizing:border-box}html,body{margin:0;width:896px;height:504px;overflow:hidden}
body{background:url(data:image/png;base64,${source}) center/cover;font-family:"Segoe UI",system-ui,sans-serif;color:var(--inkPrimary)}
.veil{position:absolute;inset:0;mask-image:linear-gradient(180deg,#000 0%,#000 60%,transparent 100%);background:linear-gradient(90deg,${palette.veil}F5 0%,${palette.veil}F0 40%,${palette.veil}B8 55%,${palette.veil}00 80%)}
.copy{position:absolute;left:50px;top:54px;text-shadow:0 3px 10px rgba(0,0,0,.75)}
h1,p{margin:0}h1{font-size:46px;font-weight:600;line-height:1.1;letter-spacing:0;width:max-content}
.conn{font-size:.65em}
.rule{width:58px;height:3px;background:var(--accent);margin-top:20px;margin-bottom:16px}
.summary{font-size:21px;font-weight:400;line-height:1.45;width:430px;letter-spacing:0}
.badge{position:absolute;right:0;top:0;width:80px;height:80px;background:var(--accent);clip-path:polygon(0 0,100% 0,100% 100%)}
.version{position:absolute;left:869px;top:27px;transform:translate(-50%,-50%) rotate(45deg);font-size:26px;font-weight:700;line-height:1;color:var(--badgeInk)}
</style><div class="veil"></div><div class="copy"><h1>Never Out <span class="conn">of</span> Print</h1><div class="rule"></div><p class="summary">Copy the books you own, and print the ones that argue.</p></div><div class="badge"></div><div class="version">${version}</div>`;
fs.writeFileSync(path.join(__dirname, 'preview-composition.html'), html);
function lum(hex) { const c = hex.map(v => v / 255).map(v => v <= .04045 ? v / 12.92 : ((v + .055) / 1.055) ** 2.4); return c[0] * .2126 + c[1] * .7152 + c[2] * .0722; }
const rgb = h => h.match(/[a-f0-9]{2}/gi).map(v => parseInt(v, 16));
const contrast = (a, b) => (Math.max(a, b) + .05) / (Math.min(a, b) + .05);
(async () => {
  const browser = await chromium.launch({ executablePath: 'C:/Program Files/Google/Chrome/Application/chrome.exe', headless: true });
  const page = await browser.newPage({ viewport: { width: 896, height: 504 }, deviceScaleFactor: 1 });
  await page.setContent(html); await page.evaluate(() => document.fonts.ready);
  await page.evaluate(() => Promise.all([...document.images].map(i => i.decode())));
  const font = await page.evaluate(() => document.fonts.check('46px "Segoe UI"'));
  const boxes = await page.evaluate(() => Object.fromEntries(['h1', '.summary'].map(s => { const b = document.querySelector(s).getBoundingClientRect(); return [s, { x: b.x, y: b.y, width: b.width, height: b.height }]; })));
  await page.screenshot({ path: path.join(root, 'Mod/About/Preview.png') });
  await page.addStyleTag({ content: '.copy{visibility:hidden}.version{visibility:hidden}' });
  const bg = await page.screenshot({ path: path.join(qa, 'background.png') });
  const { data, info } = await sharp(bg).removeAlpha().raw().toBuffer({ resolveWithObject: true });
  const checks = {}; for (const [selector, b] of Object.entries(boxes)) {
    const ink = lum(rgb(palette.inkPrimary)); let min = 100;
    for (let y = Math.floor(b.y); y < Math.ceil(b.y + b.height); y++) for (let x = Math.floor(b.x); x < Math.ceil(b.x + b.width); x++) { const i = (y * info.width + x) * info.channels; min = Math.min(min, contrast(ink, lum([...data.subarray(i, i + 3)]))); } checks[selector] = min;
  }
  checks.badge = contrast(lum(rgb(palette.badgeInk)), lum(rgb(palette.accent)));
  await sharp(path.join(root, 'Mod/About/Preview.png')).resize({ width: 268 }).png().toFile(path.join(qa, 'Preview-small.png'));
  const result = { font: 'Segoe UI', fontAvailable: font, version, boxes, minimumContrastAcrossTextRectangles: checks, bytes: fs.statSync(path.join(root, 'Mod/About/Preview.png')).size };
  fs.writeFileSync(path.join(qa, 'visual-checks.json'), JSON.stringify(result, null, 2)); console.log(JSON.stringify(result, null, 2)); await browser.close();
  if (!font || Object.values(checks).some(x => x < 4.5) || result.bytes >= 900000) throw Error('Visual check failed');
})().catch(e => { console.error(e); process.exit(1); });
