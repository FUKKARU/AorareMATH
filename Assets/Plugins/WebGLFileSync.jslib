mergeInto(LibraryManager.library, {
  JS_FileSystem_Sync: function () {
    FS.syncfs(false, function (err) {
      if (err) {
        console.error("FS.syncfs failed:", err);
      } else {
        console.log("FS.syncfs completed");
      }
    });
  }
});
