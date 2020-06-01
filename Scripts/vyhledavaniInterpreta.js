//po uvolnění tlačítka při psaní vyhledávané přezdívky
function vyhledavaniInterpreta() {

    let hodnota = $('#myInput').val();

    jQuery.ajax({
        type: 'POST',
        //dataType: 'json',
        url: "/Interpret/VyhledaniIntepreta",
        data: {
            nazevInterpreta: $('#myInput').val()
        }              
    })
        .done(function (view) {
            $("body").html(view);
            $('#myInput').val(hodnota)
            $('#myInput').focus();
    });

}