﻿function ShowLoader()
{ $(".customLoader").fadeIn("slow"); }


function HideLoader()
{ $(".customLoader").fadeOut("slow"); }



function OpenGWAModelPopup(modelTitle, Body) {
    $('#modalTitle').text(modelTitle);

    $('#modalBody').html('');
    $('#modalBody').html(Body);

    $('#myModal').modal('show');
}

function CloseGWAModelPopup(id) {
   
    $('#' + id).modal('hide');
    
}



function BindDropDown(filterData, dropdownID) {
    var url = "/Home/GetRecordsForSelection";
    dropdownID.html('');
    $.get(url, filterData, function (data) {
        if (data.Result == 200 && data.Data.length > 0) {
            $.each(data.Data, function () {
                dropdownID.append($("<option/>").val(this.Value).text(this.Text));
            });
        }
    });
}




function DisableElement(item) {
    if (item != undefined) {
        item.disabled = true;
    }
}
function EnableElement(item) {
    if (item != undefined) {
        item.disabled = false;
    }
}


