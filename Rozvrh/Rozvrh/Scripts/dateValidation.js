function isDate(txtDate) {
    var currVal = txtDate;
    if (currVal == '')
        return false;

    //Declare Regex  
    var rxDatePattern = /^(\d{1,2})(.)(\d{1,2})(.)(\d{4})$/;
    var dtArray = currVal.match(rxDatePattern); // is format OK?

    if (dtArray == null)
        return false;

    //Checks for dd.mm.yyyy format.
    dtDay = dtArray[1];
    dtMonth = dtArray[3];
    dtYear = dtArray[5];

    if (dtMonth < 1 || dtMonth > 12)
        return false;
    else if (dtDay < 1 || dtDay > 31)
        return false;
    else if ((dtMonth == 4 || dtMonth == 6 || dtMonth == 9 || dtMonth == 11) && dtDay == 31)
        return false;
    else if (dtMonth == 2) {
        var isleap = (dtYear % 4 == 0 && (dtYear % 100 != 0 || dtYear % 400 == 0));
        if (dtDay > 29 || (dtDay == 29 && !isleap))
            return false;
    }
    return true;
}

function isValidDate() {
    var inputs = document.getElementsByClassName("txtDate");
    for (var index = 0; index < inputs.length; ++index) {
        if (!isDate(inputs[index].value)) {
            inputs[index].focus();
            alert('Špatný formát data! Prosím oprav zvýrazněné pole.');
            return false;
        }
    }
    alert('Hodnoty uloženy.');
    return true;

}

function niceButton () {
    $(".exButton")
      .button();
}