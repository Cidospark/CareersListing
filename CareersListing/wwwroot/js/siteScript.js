//alert("Yes")
const btnBack = document.querySelector("#backBtn");

btnBack.addEventListener("click", function () {
    window.history.back();
});


function delConfirmation(id, tog) {
    if (tog) {
        $("#delBtn_" + id).hide();
        $("#conf_" + id).show("slow");
    } else {
        $("#delBtn_" + id).show("slow");
        $("#conf_" + id).hide("slow");
    }
}