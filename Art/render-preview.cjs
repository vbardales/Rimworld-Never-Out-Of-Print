const fs=require('fs'),path=require('path');
const {chromium}=require('C:/Users/nelim/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/playwright');
process.chdir(path.join(__dirname,'..'));
// Icon-badge corner: bottom-left (default), bottom-right, top-left. Not top-right: the '1.6' triangle badge
// already lives there. Sign of translate/rotate mirrors which edges the badge hangs off.
const ICON_BADGE_CORNERS={
  'bottom-left':{css:'left:0;bottom:0',translate:'-12.5%,12.5%',rotate:15},
  'bottom-right':{css:'right:0;bottom:0',translate:'12.5%,12.5%',rotate:-15},
  'top-left':{css:'left:0;top:0',translate:'-12.5%,-12.5%',rotate:-15},
};
const iconBadgeCorner=process.argv[2]||'bottom-left';
if(!ICON_BADGE_CORNERS[iconBadgeCorner])throw Error(`Unknown icon-badge corner '${iconBadgeCorner}', expected one of ${Object.keys(ICON_BADGE_CORNERS).join(', ')}`);
(async()=>{
const palette=JSON.parse(fs.readFileSync('Art/preview-palette.json','utf8').replace(/^\uFEFF/,''));
const xml=fs.readFileSync('Mod/About/About.xml','utf8');
const versions=[...xml.match(/<supportedVersions>([\s\S]*?)<\/supportedVersions>/)[1].matchAll(/<li>(\d+(?:\.\d+)+)<\/li>/g)].map(m=>m[1]);
versions.sort((a,b)=>{const x=a.split('.').map(Number),y=b.split('.').map(Number);for(let i=0;i<Math.max(x.length,y.length);i++){if((x[i]||0)!==(y[i]||0))return (x[i]||0)-(y[i]||0);}return 0;});
if(!versions.length)throw Error('No stable supported version');const version=versions.at(-1);
const rgb=palette.veil.match(/\w\w/g).map(h=>parseInt(h,16)).join(',');
const vars=Object.entries(palette).map(([k,v])=>`--${k}:${v}`).join(';');
const html=`<!doctype html><meta charset="utf-8"><style>
:root{${vars}}*{margin:0;padding:0;box-sizing:border-box}
html,body{width:896px;height:504px;overflow:hidden;font-family:"Segoe UI",system-ui,sans-serif}
body{background:url(Preview.png) center/cover}
.veil{position:absolute;inset:0;background:radial-gradient(circle at 0% 0%,rgba(${rgb},.86) 0%,rgba(${rgb},0) 74%)}
.copy{position:absolute;left:50px;top:54px;color:var(--inkPrimary);text-shadow:0 3px 10px rgba(0,0,0,.75)}
h1{font-size:46px;font-weight:600;line-height:1.1;letter-spacing:0}
.conn{font-size:.65em}
.rule{width:58px;height:3px;background:var(--accent);margin:20px 0 16px}
p{font-size:21px;font-weight:400;line-height:1.45;width:430px;letter-spacing:0;color:var(--inkPrimary)}
.badge{position:absolute;right:0;top:0;width:80px;height:80px;background:var(--accent);clip-path:polygon(0 0,100% 0,100% 100%)}
.version{position:absolute;left:869px;top:27px;transform:translate(-50%,-50%) rotate(45deg);font-size:26px;font-weight:700;line-height:1;color:var(--badgeInk)}
/* ModIcon-badge.png is a pre-baked asset (Make-PreviewBadge.ps1 -SaveTrimmedIconTo): the shipped ModIcon.png's
   near-black background flood-filled to transparent from its border only (a plain color-key would also blank
   matching pixels inside the artwork), then cropped to its own alpha bounding box (no dead transparent margin).
   That PNG->PNG step needs System.Drawing; this pipeline has no Node image library, so it stays a one-off
   asset here rather than a per-render step. Placement below is plain CSS: width fixes the box, height follows
   the asset's own aspect ratio; left/bottom flush it into the corner; translate(%,%) is a fraction of the
   badge's OWN box (not the page), rotate happens after, around the default center transform-origin.
*/
.icon-badge{position:absolute;${ICON_BADGE_CORNERS[iconBadgeCorner].css};width:220px;transform:translate(${ICON_BADGE_CORNERS[iconBadgeCorner].translate}) rotate(${ICON_BADGE_CORNERS[iconBadgeCorner].rotate}deg)}
</style><div class="veil"></div><div class="copy"><h1>Never Out <span class="conn">of</span> Print</h1><div class="rule"></div><p>Copy the books you own, and print the ones that argue.</p></div><div class="badge"></div><div class="version">${version}</div><img class="icon-badge" src="ModIcon-badge.png">`;
fs.writeFileSync('Art/Preview-layout.html',html);
const browser=await chromium.launch({executablePath:'C:/Program Files/Google/Chrome/Application/chrome.exe',headless:true});
const page=await browser.newPage({viewport:{width:896,height:504},deviceScaleFactor:1});
await page.goto(require('url').pathToFileURL(path.resolve('Art/Preview-layout.html')).href);await page.evaluate(()=>document.fonts.ready);
const cdp=await page.context().newCDPSession(page);await cdp.send('DOM.enable');await cdp.send('CSS.enable');const {root}=await cdp.send('DOM.getDocument');
const qa={version,paletteSource:'Art/preview-palette.json',iconBadgeCorner,elements:{}};
for(const selector of ['h1','p','.version']){
const {nodeId}=await cdp.send('DOM.querySelector',{nodeId:root.nodeId,selector});const fonts=await cdp.send('CSS.getPlatformFontsForNode',{nodeId});
qa.elements[selector]={bounds:await page.locator(selector).boundingBox(),fonts:fonts.fonts};console.log(selector,JSON.stringify(fonts.fonts));if(fonts.fonts.some(f=>!f.familyName.startsWith('Segoe UI')))throw Error('Unexpected fallback font');}
await page.screenshot({path:'Mod/About/Preview.png'});
await page.addStyleTag({content:'h1,p,.version{visibility:hidden}'});await page.screenshot({path:'Art/Preview-background-qa.png'});
fs.writeFileSync('Art/Preview-qa.json',JSON.stringify(qa,null,2));await browser.close();
})();
