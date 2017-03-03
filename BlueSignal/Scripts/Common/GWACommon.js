function ShowLoader()
{ $(".customLoader").fadeIn("slow"); }


function HideLoader()
{ $(".customLoader").fadeOut("slow"); }

function ShowLoaderCustom() {
    $('#loading').modal('show');
    //$('#ModelBackGround_Custom').show(500);

    $('#loading').show();
    $('#ModelBackGround_Custom').show();
    
}

function HideLoaderCustom() {
    $('#loading').hide();
    $('#loading').modal('hide');
    $('#ModelBackGround_Custom').hide();
    $('.modal-backdrop').hide();
    
}

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


/*****************/
function AddContact(btn) {

    if (btn != undefined)
    {
        btn.disabled = true;
    }

    var data = {
        'Name': $('#Name').val(),
        'Message': $('#Message').val(),
        'Subject': $('#Subject').val(),
        'Email': $('#Email').val()
    };

    $.ajax({
        type: "Post",
        url: "/Home/Contact",
        dataType: "html",
        async: true,
        data: data,
        success: function (data) {
            if (data != null) {
                $('#ContactLog_Container').html('');
                $('#ContactLog_Container').html(data);

                btn.disabled = false;
            }
        },
        error: function (error) {
            //console.log(error);
            btn.disabled = false;
            //alert('Error');
        }
    });
}



