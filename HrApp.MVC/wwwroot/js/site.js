// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Theme toggle (light/dark) with persistence
(function () {
  var storageKey = 'hrapp.theme';
  var root = document.documentElement;
  function apply(theme) {
    if (theme === 'light') {
      root.setAttribute('data-theme', 'light');
    } else if (theme === 'dark') {
      root.removeAttribute('data-theme');
    }
  }
  try {
    var saved = localStorage.getItem(storageKey);
    if (saved === 'light' || saved === 'dark') {
      apply(saved);
    }
  } catch (e) { /* no-op */ }

  document.addEventListener('DOMContentLoaded', function () {
    var btn = document.getElementById('themeToggle');
    if (!btn) return;
    btn.addEventListener('click', function () {
      var current = root.getAttribute('data-theme') === 'light' ? 'light' : 'dark';
      var next = current === 'light' ? 'dark' : 'light';
      apply(next);
      try { localStorage.setItem(storageKey, next); } catch (e) {}
    });
  });
})();
