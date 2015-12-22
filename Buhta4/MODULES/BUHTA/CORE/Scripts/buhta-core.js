

//if (window.opener) {
//    window.name = "win" + Math.random().toString().replace(".", "");
//}
//else {
//    window.name = "parent";
//}

var bindingHub;
var signalr = {};
bindingHub = $.connection.bindingHub;
$.connection.hub.start().done();

var _docReady = [];
var docReady = function (callback) { _docReady.push(callback); };

$(document).ready(function () {
    //$(window).on('unload', function () {
    //    alert('beforeunload-1');
    //    bindingHub.server.unloadChromeWindow(localStorage.ChromeSessionId, window.name);
    //});

    $.connection.hub.start().done(function () {
        bindingHub.server.registerChromeWindow(localStorage.ChromeSessionId, window.name, window._buhtaModelBindingId, window._buhtaRecordId);

        for (i = 0; i < _docReady.length; i++) {
            _docReady[i]();
        };
    })

});


signalr.bindedValueChangedListeners = [];

signalr.subscribeModelPropertyChanged = function (modelBindingId, propertyName, callBack) {
    docReady(function () {
        bindingHub.server.subscribeBindedValueChanged(localStorage.ChromeSessionId, modelBindingId, propertyName);
        //alert("hub.start():" + propertyName);
        //console.log('subscribeBindedValueChanged**', modelBindingId, propertyName);
    });
    signalr.bindedValueChangedListeners.push({ modelBindingId: modelBindingId, propertyName: propertyName, callBack: callBack });
};

bindingHub.client.receiveBindedValueChanged = function (modelBindingId, propertyName, newValue) {
    //console.log('receiveBindedValueChanged', modelBindingId, propertyName, newValue);
    for (var i = 0; i < signalr.bindedValueChangedListeners.length; i++) {
        var listener = signalr.bindedValueChangedListeners[i];
        if (listener.modelBindingId == modelBindingId && listener.propertyName == propertyName) {
            listener.callBack(newValue);
            //console.log('receiveBindedValueChanged.callBack', modelBindingId, propertyName, newValue);
        }
    }
};

bindingHub.client.receiveBindedValuesChanged = function (modelBindingId, values) {
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
    console.log(script);
    eval(script);
};


////$(window).unl unload(function () {
//$(window).on('beforeunload', function () {
//    if (window.opener) {
//        window.open('', 'parent').focus();
//    }
//})


var buhta = {};

// -----------  FancyTree  -----------------
buhta.FancyTree = {};

buhta.FancyTree.convertFlatDataToTree = function (childList) {
    var parent,
        nodeMap = {};
    $.each(childList, function (i, c) {
        nodeMap[c.id] = c;
    });
    childList = $.map(childList, function (c) {
        c.key = c.id;
        delete c.id;
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



// -----------  DataTables.net  -----------------
buhta.DataTables = {};

buhta.DataTables.SelectRowById = function (tableId, keyColumnName, id) {
    var table = $("#" + tableId).DataTable();
    var colIndex = table.column(keyColumnName + ":name").index()

    table.rows().every(function (rowIdx, tableLoop, rowLoop) {
        var data = this.data();
        if (data[colIndex] == id)
            this.select();
        else
            this.deselect();
    });
}


buhta.setBrowserTabFocused = function () {
    document.body.setAttribute('data-buhta-focus-me-2128506', 'ok');
    if (document.body.getAttribute('data-buhta-chrome-ext') == 'ok') {
        setTimeout(function () {
            if (document.body.getAttribute('data-buhta-focus-me-2128506') == 'ok')
                alert(' ');
        }, 250);
    }
    else {
        alert(' ');
    }
}

