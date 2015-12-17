
setInterval(function () {
    chrome.tabs.query({}, function (tabs) {
        for (var i = 0; i < tabs.length; i++) {
            var tab = chrome.tabs.get(tabs[i].id, function (tab) {
                if (tab.url.indexOf('chrome://') < 0) {
                    chrome.tabs.executeScript(tab.id, { code: 'document.body.setAttribute("data-buhta-chrome-ext","ok"); document.body.getAttribute("data-buhta-focus-me-2128506");' }, function (result) {
                        if (result && result[0] == "ok") {
                            setTimeout(function () {
                                chrome.tabs.update(tab.id, { selected: true });
                                console.log(' set focus -> ' + result[0] + ":" + tab.id);
                            }, 1);
                            setTimeout(function () {
                                chrome.tabs.executeScript(tab.id, { code: 'document.body.setAttribute("data-buhta-focus-me-2128506","no");' }, function (result) { });
                            }, 10);

                        };
                    });

                }
            });
        }
    });
}, 200);

