// Composes Mod/About/Preview.png from the textures the mod actually ships.
const fs = require('fs');
const path = require('path');
const base = path.join(__dirname, '..');
const W = 896, H = 504;
const uri = (rel) => 'data:image/png;base64,' + fs.readFileSync(path.join(base, 'Mod/Textures', rel)).toString('base64');
const img = (rel, x, y, w, h) => `<image href="${uri(rel)}" x="${x}" y="${y}" width="${w}" height="${h}" preserveAspectRatio="xMidYMid meet"/>`;
const text = (x, y, s, size, fill, weight = 'normal', anchor = 'start') =>
  `<text x="${x}" y="${y}" font-family="Segoe UI, Arial, sans-serif" font-size="${size}" font-weight="${weight}" fill="${fill}" text-anchor="${anchor}">${s}</text>`;
const svg = [
  `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 ${W} ${H}" width="${W}" height="${H}">`,
  `<rect width="${W}" height="${H}" fill="#23211E"/>`,
  `<rect x="0" y="0" width="${W}" height="118" fill="#2E2B26"/>`,
  text(40, 62, 'NEVER OUT OF PRINT', 44, '#F0E7D5', '600'),
  text(42, 94, 'Copy the books you own. Print the ones that argue.', 19, '#A79C86'),
  img('Things/Building/Production/PrintingPressElectric_south.png', 40, 142, 340, 179),
  text(40, 352, 'Electric press', 20, '#F0E7D5', '600'),
  text(40, 376, 'Four times the speed, for a price in power.', 15, '#8E8577'),
  img('Things/Building/Production/PrintingPressManual_south.png', 450, 150, 200, 158),
  text(550, 352, 'Manual press', 20, '#F0E7D5', '600', 'middle'),
  text(550, 376, 'Medieval. Wood or metal.', 15, '#8E8577', 'normal', 'middle'),
  img('Things/Item/NeverOutOfPrint_IdeoligionBook.png', 715, 178, 130, 130),
  text(780, 352, 'Ideoligion book', 20, '#F0E7D5', '600', 'middle'),
  text(780, 376, 'Reading it moves certainty.', 15, '#8E8577', 'normal', 'middle'),
  `<rect x="40" y="424" width="816" height="1" fill="#3D3934"/>`,
  text(40, 456, 'A copy teaches exactly what the original teaches, and sells for a tenth.', 16, '#8E8577'),
  '</svg>',
].join('\n');
fs.mkdirSync(path.join(base, '_tools/svg'), { recursive: true });
fs.writeFileSync(path.join(base, '_tools/svg/Preview.svg'), svg);
console.log('wrote _tools/svg/Preview.svg');
