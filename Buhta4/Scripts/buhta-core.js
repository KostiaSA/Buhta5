var bindingHub;
bindingHub = $.connection.bindingHub;

var signalr = {};

signalr.bindedValueChangedListeners = [];

signalr.subscribeModelPropertyChanged = function (modelBindingId, propertyName, callBack) {
  $.connection.hub.start().done(function() {
      $.connection.bindingHub.server.subscribeBindedValueChanged(modelBindingId, propertyName);
      //console.log('subscribeBindedValueChanged**', modelBindingId, propertyName);
  });
  signalr.bindedValueChangedListeners.push({ modelBindingId: modelBindingId, propertyName: propertyName, callBack: callBack });
};

$.connection.bindingHub.client.receiveBindedValueChanged = function (modelBindingId, propertyName, newValue) {
    //console.log('receiveBindedValueChanged', modelBindingId, propertyName, newValue);
    for (var i = 0; i < signalr.bindedValueChangedListeners.length; i++) {
        var listener=signalr.bindedValueChangedListeners[i];
        if (listener.modelBindingId == modelBindingId && listener.propertyName == propertyName) {
            listener.callBack(newValue);
            //console.log('receiveBindedValueChanged.callBack', modelBindingId, propertyName, newValue);
        }
    }
};

$.connection.bindingHub.client.receiveBindedValuesChanged = function (modelBindingId, values) {
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

$.connection.bindingHub.client.receiveServerError = function (error) {
    console.error(error);
    $("body").html("<p style='color:red;padding-left: 10px;'>" + error.replace(/\n/g, '<br/>') + "</p>").css("color:red");
};

$.connection.bindingHub.client.receiveShowWindow = function (windowHtml) {
    //var win = $("<div>"+windowHtml+"</div>").appendTo("#popups");
    var win = $("<div><div></div></div>").appendTo("#popups");

    $(win).jqxWindow({
        content: windowHtml,
        isModal:true,
        showCollapseButton: true, maxHeight: 400, maxWidth: 700, minHeight: 10, minWidth: 400,// height: 300, width: 500,
        initContent: function () {
            //$('#tab').jqxTabs({ height: '100%', width: '100%' });
            $(win).jqxWindow('focus');
        }
    });
};

$.connection.bindingHub.client.receiveScript = function (script) {
    eval(script);
};

