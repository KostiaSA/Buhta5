//if (window.opener) {
//    alert('opener');
//    if (window.opener.bindingHub) {
//        alert('opener.bindingHub');
//    }
//}

var isOpera = !!window.opera || navigator.userAgent.indexOf(' OPR/') >= 0;
var isFirefox = typeof InstallTrigger !== 'undefined';   // Firefox 1.0+
var isSafari = Object.prototype.toString.call(window.HTMLElement).indexOf('Constructor') > 0;
var isChrome = !!window.chrome && !isOpera;              // Chrome 1+
var isIE = /*@cc_on!@*/false || !!document.documentMode; // At least IE6

var bindingHub;
var signalr = { wins: [] };

//if (window.opener && window.opener.bindingHub) {
//    bindingHub = window.opener.bindingHub;
//    signalr = window.opener.signalr;
//    window.name = "win" + Math.random().toString().replace(".", "");
//    signalr.wins[window.name] = window;
//}
//else {
if (window.opener) {
    window.name = "win" + Math.random().toString().replace(".", "");
}
else {
    window.name = "parent";
}

signalr.wins[window.name] = window;
bindingHub = $.connection.bindingHub;


signalr.bindedValueChangedListeners = [];

signalr.subscribeModelPropertyChanged = function (chromeWindowId, modelBindingId, propertyName, callBack) {
    $.connection.hub.start().done(function () {
        $.connection.bindingHub.server.subscribeBindedValueChanged(chromeWindowId, modelBindingId, propertyName);
        //alert("hub.start():" + propertyName);
        //console.log('subscribeBindedValueChanged**', modelBindingId, propertyName);
    });
    signalr.bindedValueChangedListeners.push({ chromeWindowId: chromeWindowId, modelBindingId: modelBindingId, propertyName: propertyName, callBack: callBack });
};

$.connection.bindingHub.client.receiveBindedValueChanged = function (chromeWindowId, modelBindingId, propertyName, newValue) {
    //console.log('receiveBindedValueChanged', modelBindingId, propertyName, newValue);
    for (var i = 0; i < signalr.bindedValueChangedListeners.length; i++) {
        var listener = signalr.bindedValueChangedListeners[i];
        if (listener.modelBindingId == modelBindingId && listener.propertyName == propertyName) {
            listener.callBack(newValue);
            //console.log('receiveBindedValueChanged.callBack', modelBindingId, propertyName, newValue);
        }
    }
};

$.connection.bindingHub.client.receiveBindedValuesChanged = function (chromeWindowId, modelBindingId, values) {
    //console.log('receiveBindedValueChanged', modelBindingId, propertyName, newValue);
    for (var i = 0; i < signalr.bindedValueChangedListeners.length; i++) {
        var listener = signalr.bindedValueChangedListeners[i];
        if (listener.modelBindingId == modelBindingId) {
            if (values.hasOwnProperty(listener.propertyName)) {
                listener.callBack(values[listener.propertyName]);
            }
            //console.log('receiveBindedValueChanged.callBack', modelBindingId, propertyName, newValue);
        }
    }
};

$.connection.bindingHub.client.receiveServerError = function (chromeWindowId, error) {
    if (chromeWindowId in signalr.wins) {
        signalr.wins[chromeWindowId].console.error(error);
        signalr.wins[chromeWindowId].$("body").html("<p style='color:red;padding-left: 10px;'>" + error.replace(/\n/g, '<br/>') + "</p>").css("color:red");
    }
};

//$.connection.bindingHub.client.receiveShowWindow = function (windowHtml) {
//    //var win = $("<div>"+windowHtml+"</div>").appendTo("#popups");
//    var win = $("<div><div></div></div>").appendTo("#popups");

//    $(win).jqxWindow({
//        content: windowHtml,
//        isModal: true,
//        showCollapseButton: true, maxHeight: 400, maxWidth: 700, minHeight: 10, minWidth: 400,// height: 300, width: 500,
//        initContent: function () {
//            //$('#tab').jqxTabs({ height: '100%', width: '100%' });
//            $(win).jqxWindow('focus');
//        }
//    });
//};

$.connection.bindingHub.client.receiveScript = function (chromeWindowId, script) {
    if (chromeWindowId in signalr.wins)
        signalr.wins[chromeWindowId].eval(script);
}
;


//$(window).unl unload(function () {
$(window).on('beforeunload', function () {
    if (window.opener) {
        window.open('', 'parent').focus();
    }
})

$(window).on('unload', function () {
    delete signalr.wins[window.name];
})
