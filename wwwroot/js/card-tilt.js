/**
 * Mouse-follow 3D card tilt (CSS transforms only — no WebGL / Three.js).
 * Bind any element with [data-card-tilt]. Optional attrs:
 *   data-tilt-max      max degrees from rest (default 12)
 *   data-tilt-rest-rx  rest rotateX degrees (default 4)
 *   data-tilt-rest-ry  rest rotateY degrees (default -6)
 */
(() => {
    const reduced = window.matchMedia('(prefers-reduced-motion: reduce)');

    function num(el, name, fallback) {
        const v = parseFloat(el.getAttribute(name) ?? '');
        return Number.isFinite(v) ? v : fallback;
    }

    function setVars(el, rx, ry, gx, gy, go) {
        el.style.setProperty('--tilt-rx', `${rx}deg`);
        el.style.setProperty('--tilt-ry', `${ry}deg`);
        el.style.setProperty('--glare-x', `${gx}%`);
        el.style.setProperty('--glare-y', `${gy}%`);
        el.style.setProperty('--glare-o', String(go));
    }

    function reset(el) {
        const restRx = num(el, 'data-tilt-rest-rx', 4);
        const restRy = num(el, 'data-tilt-rest-ry', -6);
        el.classList.remove('is-tilting');
        setVars(el, restRx, restRy, 50, 40, 0);
    }

    function onMove(e) {
        if (reduced.matches) return;
        const el = e.currentTarget;
        const rect = el.getBoundingClientRect();
        if (rect.width < 1 || rect.height < 1) return;

        const px = (e.clientX - rect.left) / rect.width;
        const py = (e.clientY - rect.top) / rect.height;
        const nx = Math.max(-1, Math.min(1, (px - 0.5) * 2));
        const ny = Math.max(-1, Math.min(1, (py - 0.5) * 2));

        const max = num(el, 'data-tilt-max', 12);
        const restRx = num(el, 'data-tilt-rest-rx', 4);
        const restRy = num(el, 'data-tilt-rest-ry', -6);

        // Natural: move mouse up → tip top edge back; mouse right → tip right edge back.
        const rx = restRx + (-ny * max);
        const ry = restRy + (nx * max);

        el.classList.add('is-tilting');
        setVars(el, rx, ry, px * 100, py * 100, 0.55);
    }

    function onLeave(e) {
        reset(e.currentTarget);
    }

    function bind(el) {
        if (!(el instanceof HTMLElement) || el.dataset.tiltBound === '1') return;
        el.dataset.tiltBound = '1';
        reset(el);
        el.addEventListener('pointermove', onMove);
        el.addEventListener('pointerleave', onLeave);
        el.addEventListener('pointercancel', onLeave);
    }

    function scan(root) {
        if (!root || root.nodeType !== 1 && root.nodeType !== 9) return;
        if (root.matches?.('[data-card-tilt]')) bind(root);
        root.querySelectorAll?.('[data-card-tilt]').forEach(bind);
    }

    function start() {
        scan(document);
        const mo = new MutationObserver((mutations) => {
            for (const m of mutations) {
                for (const n of m.addedNodes) {
                    if (n.nodeType === 1) scan(n);
                }
            }
        });
        mo.observe(document.body, { childList: true, subtree: true });
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', start);
    } else {
        start();
    }
})();
