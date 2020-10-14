//Refresh DataTable for specific row
function refreshDatatable(tableId, rowId) {
    $(tableId).DataTable().row(`[data-RowId='${rowId}']`).remove().draw(false);
}

//Close Modal
function closeModal() {
    $("#modalContent").html("");
    $("#divModal").modal("hide");
}

//Loader
$(document).ajaxStart(function () { $.LoadingOverlay("show"); });
$(document).ajaxStop(function () { $.LoadingOverlay("hide"); });


//UserName ValidationRule
jQuery.validator.addMethod("alphanumeric", function (value, element) {
    return this.optional(element) || /^(?=.*[0-9])(?=.*[a-zA-Z])([a-zA-Z0-9]+)$/i.test(value);
}, "Please enter combination of characters and numbers");

//Number ValidationRule
jQuery.validator.addMethod("numeric", function (value, element) {
    return this.optional(element) || /^(?=.*[0-9])([0-9]+)$/i.test(value);
}, "Please enter numbers");


//SubscriptionTypes
SubscriptionTypes = {
    Annually_and_Normal: 1,
    Annually_and_Plus: 2,
    Monthly_and_Normal: 3,
    Monthly_and_Plus: 4
};

//DataTable ActionMenu
function dataTableAction(menu,link) {

    
  //  var content = `<div class=""  title="">
  //  <button type="button" class="btn dropdown-toggle " data-toggle="dropdown">
  //      <i class="fa fa-circle-o" aria-hidden="true"></i>
  //      <i class="fa fa-circle-o" aria-hidden="true"></i>
  //      <i class="fa fa-circle-o" aria-hidden="true"></i>
  //  </button>
  //  <div class="dropdown-menu">{MENU}</div>
  //</div>`;

  var content = `<div class=""  title="">
    <a href="javascript:;" class="btn dropdown-toggle " data-toggle="dropdown">
       ${link}
    </a>
    <div class="dropdown-menu">{MENU}</div>
  </div>`;
    return content.replace("{MENU}", menu);
}

// isActive Toggle
function statusToggle(id, status, isDisabled = false) {
    var content = `<div class="custom-control custom-switch">
  <input type="checkbox" class="custom-control-input" id="ck${id}" {checked} {disabled} data-id="{id}">
  <label class="custom-control-label" for="ck${id}"></label>
</div>`;


    content = content.replace("{id}", id);
    content = content.replace("{checked}", status === true ? "checked" : "");
    content = content.replace("{disabled}", isDisabled === true ? "disabled" : "");
    return content;
}

//remove element from javascript array
function arrayRemove(array, element) {
    var index = array.indexOf(element);
    array.splice(index, 1);
    return array;
}
