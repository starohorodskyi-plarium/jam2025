mergeInto(LibraryManager.library, {
  WGGetPlatformInfo: function () {
    var json = '{}';
    try {
      if (typeof window.wgGetPlatformInfo === 'function') {
        json = window.wgGetPlatformInfo();
      }
    } catch (e) {}
    var size = lengthBytesUTF8(json) + 1;
    var buffer = _malloc(size);
    stringToUTF8(json, buffer, size);
    return buffer;   // Unity сам смаршалит указатель в string
  }
});
