setInterval(function () {
    chrome.tabs.query({}, function (tabs) {
        for (var i = 0; i < tabs.length; i++) {
            var x = tabs[i].id;
            console.log(x);
            //chrome.tabs.executeScript(tabs[i].id, { code: 'var x=document.getElementById("buhta-focus-me-2128506"); if (x) { setTimeout(function () { var el = document.getElementById("buhta-focus-me-2128506"); el.parentElement.removeChild(el); }, 10); "ok";} else { null; }' }, function (result) {
            chrome.tabs.executeScript(tabs[i].id, { code: 'var z=document.getElementById("buhta-focus-me-2128506"); if (z) {"ok";} else { null;}' }, function (result) {
                if (result && result[0] != null) {
                    chrome.tabs.update(x, { selected: true });
                    console.log(result[0] + ":" + x);
                }
            });
        }
    });
}, 500);

////chrome.runtime.onMessage.addListener(
////  function (request, sender, sendResponse) {
////      sendResponse();
////      //if (request == "buhta-focus-me")
////        //  chrome.tabs.update(sender.tab.id, { selected: true });
////  });


