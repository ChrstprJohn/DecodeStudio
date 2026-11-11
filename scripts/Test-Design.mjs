import assert from 'node:assert/strict';
import { readFileSync } from 'node:fs';
import vm from 'node:vm';

const events = {};
const track = {
    children: Array(16),
    firstElementChild: { getBoundingClientRect() { return { height: 200 }; } },
    addEventListener(name, handler) { events['track-' + name] = handler; }
};
const gallery = {
    querySelector() { return track; },
    addEventListener(name, handler) { events['gallery-' + name] = handler; },
    contains(target) { return target === track; }
};
const media = { matches: false, addEventListener(name, handler) { events.media = handler; } };
const summary = {
    focus() { events.focused = true; },
    setAttribute(name, value) { events[name] = value; },
    addEventListener(name, handler) { events.menuToggle = handler; }
};
const nav = { children: [], querySelector() { return null; }, querySelectorAll(selector) {
    return selector === 'a' ? [{ addEventListener(name, handler) { events.menuLink = handler; } }] : [];
} };
const pageSection = { inert: false };
const menu = {
    open: false,
    closest() { return { prepend() {}, querySelector() { return null; }, classList: { contains() { return true; }, add() {}, remove() {} } }; },
    querySelector(selector) { return selector === 'summary' ? summary : nav; }
};
const document = {
    documentElement: { classList: { remove() {} } },
    hidden: false,
    createElement() { return { setAttribute() {} }; },
    querySelector(selector) {
        if (selector === 'header details') return menu;
        if (selector === '[data-gallery]') return gallery;
        if (selector === 'main' || selector === 'footer') return pageSection;
        return null;
    },
    addEventListener(name, handler) { events[name] = handler; }
};
let stopped = false;
let progress = 0;
let tweenVars;
const entranceTargets = [];
const entranceCalls = [];
let customCurve;
const CustomEase = { create(name, curve) { customCurve = curve; return name; } };
const gsap = {
    set() {},
    registerPlugin() {},
    matchMedia() { return { add(query, callback) { if (!media.matches) callback(); } }; },
    timeline() {
        const labels = new Map();
        return {
            addLabel(name, at) { labels.set(name, at); return this; },
            from(target, vars, at) {
                entranceTargets.push(target);
                entranceCalls.push({ target, vars, at: labels.get(at) ?? at });
                return this;
            }
        };
    },
    fromTo(target, from, to) {
    tweenVars = to;
    return {
        paused(value) { stopped = value; },
        progress(value) { if (value === undefined) return progress; progress = value; },
        kill() {}
    };
} };
const context = vm.createContext({
    document, gsap, CustomEase, window: { gsap, CustomEase, matchMedia(query) {
        return query ? { matches: query.includes('reduce'), addEventListener() {} } : media;
    } },
    IntersectionObserver: class {
        constructor(handler) { events.intersection = handler; }
        observe() {}
    },
    ResizeObserver: class {
        constructor(handler) { events.resize = handler; }
        observe() {}
    }
});
for (const file of ['site.js', 'hero-animation.js']) {
    if (file === 'hero-animation.js') context.window.matchMedia = () => media;
    vm.runInContext(readFileSync(new URL(`../MyPortfolio/wwwroot/js/${file}`, import.meta.url), 'utf8'), context);
}
assert.deepEqual(entranceTargets, ['.hero-heading', '.hero-introduction', '.home-header', '.gallery-window']);
assert.deepEqual(entranceCalls.map(call => call.at), [0.2, 0.4, 0.4, 0.6]);
assert.equal(entranceCalls[3].vars.duration, 1.6);
assert.equal(customCurve, '0.2,1,1,1');
assert.equal(tweenVars.x, 1600);
assert.equal(tweenVars.y, -1600);
assert.equal(tweenVars.repeat, -1);
assert.equal(stopped, false);
assert.equal(events['track-pointerenter'], undefined);
assert.equal(events['gallery-focusin'], undefined);
assert.equal(tweenVars.duration, 28);
media.matches = true;
events.media();
assert.equal(stopped, true);
media.matches = false;
events.media();
assert.equal(stopped, false);
events.intersection([{ isIntersecting: false }]);
assert.equal(stopped, true);
events.intersection([{ isIntersecting: true }]);
document.hidden = true;
events.visibilitychange();
assert.equal(stopped, true);
document.hidden = false;
events.visibilitychange();
assert.equal(stopped, false);
progress = .4;
track.firstElementChild.getBoundingClientRect = () => ({ height: 400 });
events.resize();
assert.equal(tweenVars.x, 3200);
assert.equal(progress, .4);
media.matches = true;
events.menuToggle({ preventDefault() {} });
assert.equal(menu.open, true);
assert.equal(pageSection.inert, true);
events.menuLink();
assert.equal(menu.open, false);
assert.equal(pageSection.inert, false);
events.menuToggle({ preventDefault() {} });
events.keydown({ key: 'Escape' });
assert.equal(menu.open, false);
assert.equal(events.focused, true);
console.log('PASS: diagonal loop, resize continuity, continuous motion without hover pausing, entrance order, reduced motion, visibility, and mobile menu.');

// A colored image produces a real palette; unloaded images keep the CSS fallback.
const colors = {};
const pixels = new Uint8ClampedArray([
    255,255,255,255, 48,72,48,255, 48,72,48,255,
    216,240,216,255, 216,240,216,255
]);
const coloredImage = { complete: true, naturalWidth: 100, getAttribute() { return '/uploads/images/custom.png'; } };
const fallbackImage = { complete: true, naturalWidth: 0, getAttribute() { return '/uploads/images/missing.png'; }, addEventListener() {} };
const panel = { style: { setProperty(name, value) { colors[name] = value; } } };
vm.runInNewContext(readFileSync(new URL('../MyPortfolio/wwwroot/js/hero-palette.js', import.meta.url), 'utf8'), {
    document: {
        querySelectorAll() { return [
            { querySelector(selector) { return selector === 'img' ? coloredImage : panel; } },
            { querySelector(selector) { return selector === 'img' ? fallbackImage : panel; } }
        ]; },
        createElement() { return { getContext() { return {
            drawImage() {}, getImageData() { return { data: pixels }; }
        }; } }; }
    }
});
assert.equal(colors['--project-color'], 'rgb(48,72,48)');
assert.equal(colors['--project-accent'], 'rgb(216,240,216)');
console.log('PASS: image palette sampling and unloaded-image fallback.');


