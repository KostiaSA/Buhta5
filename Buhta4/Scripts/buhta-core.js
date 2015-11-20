﻿

if (window.opener) {
    window.name = "win" + Math.random().toString().replace(".", "");
}
else {
    window.name = "parent";
}

var bindingHub;
var signalr = {};
bindingHub = $.connection.bindingHub;


signalr.bindedValueChangedListeners = [];

signalr.subscribeModelPropertyChanged = function (modelBindingId, propertyName, callBack) {
    $.connection.hub.start().done(function () {
        bindingHub.server.subscribeBindedValueChanged( modelBindingId, propertyName);
        //alert("hub.start():" + propertyName);
        //console.log('subscribeBindedValueChanged**', modelBindingId, propertyName);
    });
    signalr.bindedValueChangedListeners.push({  modelBindingId: modelBindingId, propertyName: propertyName, callBack: callBack });
};

bindingHub.client.receiveBindedValueChanged = function ( modelBindingId, propertyName, newValue) {
    //console.log('receiveBindedValueChanged', modelBindingId, propertyName, newValue);
    for (var i = 0; i < signalr.bindedValueChangedListeners.length; i++) {
        var listener = signalr.bindedValueChangedListeners[i];
        if (listener.modelBindingId == modelBindingId && listener.propertyName == propertyName) {
            listener.callBack(newValue);
            //console.log('receiveBindedValueChanged.callBack', modelBindingId, propertyName, newValue);
        }
    }
};

bindingHub.client.receiveBindedValuesChanged = function ( modelBindingId, values) {
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

bindingHub.client.receiveServerError = function (error) {
    console.error(error);
    $("body").html("<p style='color:red;padding-left: 10px;'>" + error.replace(/\n/g, '<br/>') + "</p>").css("color:red");

};


bindingHub.client.receiveScript = function (script) {
    eval(script);
};


////$(window).unl unload(function () {
//$(window).on('beforeunload', function () {
//    if (window.opener) {
//        window.open('', 'parent').focus();
//    }
//})

function convertFlatDataToFancyTree(childList) {
    var parent,
        nodeMap = {};
    $.each(childList, function (i, c) {
        nodeMap[c.id] = c;
    });
    childList = $.map(childList, function (c) {
        c.key = c.id;
        delete c.id;
        //c.selected = (c.status === "completed");
        if (c.parent) {
            parent = nodeMap[c.parent];
            if (parent.children) {
                parent.children.push(c);
            } else {
                parent.children = [c];
            }
            return null; 
        }
        return c;
    });
    $.each(childList, function (i, c) {
        if (c.children && c.children.length > 1) {
            c.children.sort(function (a, b) {
                return ((a.position < b.position) ? -1 : ((a.position > b.position) ? 1 : 0));
            });
        }
    });
    return childList;
}

