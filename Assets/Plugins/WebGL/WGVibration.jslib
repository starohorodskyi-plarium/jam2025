mergeInto(LibraryManager.library, {
  WGVibrateMs: function (ms) {
    if (typeof window.wgVibrate === 'function') {
      return window.wgVibrate(ms) ? 1 : 0;
    }
    return 0;
  },

  WGVibratePattern: function (patternPtr) {
    var pattern = UTF8ToString(patternPtr);
    if (typeof window.wgVibrate === 'function') {
      return window.wgVibrate(pattern) ? 1 : 0;
    }
    return 0;
  },

  WGVibrateStop: function () {
    if (typeof window.wgVibrateStop === 'function') {
      window.wgVibrateStop();
    }
  },

  WGCanVibrate: function () {
    return (window.WGPlatform && window.WGPlatform.canVibrate) ? 1 : 0;
  }
});
