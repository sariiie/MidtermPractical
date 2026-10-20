// Confetti: cannons from the bottom corners plus a rain from the top.
// Called from Results.razor as trivia.confetti(count).
window.trivia = {
  // localStorage helpers used by AuthService
  store: {
    get: function (key) { return window.localStorage.getItem(key); },
    set: function (key, value) { window.localStorage.setItem(key, value); },
    remove: function (key) { window.localStorage.removeItem(key); }
  },

  confetti: function (count) {
    if (window.matchMedia && window.matchMedia("(prefers-reduced-motion: reduce)").matches) return;

    count = Math.min(Math.max(count || 150, 20), 400);

    var canvas = document.createElement("canvas");
    canvas.style.cssText =
      "position:fixed;inset:0;width:100%;height:100%;pointer-events:none;z-index:9999";
    canvas.width = window.innerWidth;
    canvas.height = window.innerHeight;
    document.body.appendChild(canvas);

    var ctx = canvas.getContext("2d");
    var W = canvas.width;
    var H = canvas.height;
    var colors = ["#ffd23f", "#ff5d73", "#3ddc97", "#4cc9f0", "#fff4d6"];
    var parts = [];

    for (var i = 0; i < count; i++) {
      var p;
      if (i < count * 0.6) {
        var left = i % 2 === 0;
        var angle = ((left ? -65 : -115) * Math.PI) / 180 + (Math.random() - 0.5) * 0.7;
        var speed = 10 + Math.random() * 12;
        p = { x: left ? 0 : W, y: H, vx: Math.cos(angle) * speed, vy: Math.sin(angle) * speed };
      } else {
        p = {
          x: Math.random() * W,
          y: -20 - Math.random() * H * 0.6,
          vx: (Math.random() - 0.5) * 2,
          vy: 2 + Math.random() * 3
        };
      }
      p.size = 6 + Math.random() * 8;
      p.color = colors[Math.floor(Math.random() * colors.length)];
      p.rot = Math.random() * Math.PI * 2;
      p.vr = (Math.random() - 0.5) * 0.3;
      p.round = Math.random() < 0.3;
      parts.push(p);
    }

    var start = performance.now();
    var last = start;

    function frame(now) {
      var dt = Math.min((now - last) / 16.67, 3);
      last = now;
      ctx.clearRect(0, 0, W, H);

      var alive = 0;
      for (var j = 0; j < parts.length; j++) {
        var q = parts[j];
        q.vy = Math.min(q.vy + 0.28 * dt, 8);
        q.vx *= Math.pow(0.99, dt);
        q.x += q.vx * dt;
        q.y += q.vy * dt;
        q.rot += q.vr * dt;

        if (q.y > H + 30) continue;
        alive++;

        ctx.save();
        ctx.translate(q.x, q.y);
        ctx.rotate(q.rot);
        ctx.fillStyle = q.color;
        if (q.round) {
          ctx.beginPath();
          ctx.arc(0, 0, q.size / 2, 0, Math.PI * 2);
          ctx.fill();
        } else {
          ctx.fillRect(-q.size / 2, -q.size / 4, q.size, q.size / 2);
        }
        ctx.restore();
      }

      if (alive > 0 && now - start < 8000) {
        requestAnimationFrame(frame);
      } else {
        canvas.remove();
      }
    }

    requestAnimationFrame(frame);
  }
};
