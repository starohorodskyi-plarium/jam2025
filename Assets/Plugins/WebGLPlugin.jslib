mergeInto(LibraryManager.library, {
    EngineLoaded: function () {
        if (typeof engineLoaded === "function") {
            engineLoaded();
        } else {
            console.log("engineLoaded not found!");
        }
    }
});