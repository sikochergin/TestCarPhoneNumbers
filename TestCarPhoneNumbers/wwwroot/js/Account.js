function independ(carn, phonen) {
    var phone = phonen.replace(/\D/g, '')
    var car = carNumberVal(carn)
    $.ajax({
        method: 'post',
        url: '/DependenceRegister/DeleteDependence?phone=' + phone + '&car=' + car,
        error: function (result) {
            console.log(result);

            $.toast({
                text: 'Ошибка',
                showHideTransition: 'slide',
                icon: 'error',
                position: 'top-right',
            });
        },
        success: function (result) {
            if (result.status) {
                $.toast({
                    text: "Номер отвязан",
                    showHideTransition: 'slide',
                    icon: 'success',
                    position: 'top-right',
                });
                buttonUsless()
                setTimeout(function () {
                    window.location.href = '/Account';
                }, 1500);
            }
            else {
                $.toast({
                    text: result.message,
                    showHideTransition: 'slide',
                    icon: 'error',
                    position: 'top-right',
                });
            }
        }
    })
}

function carNumberVal(numb) {
    var le = numb.length;
    if (le > 9 || le < 7) {
        return ""
    }
    const allowLetters = "ABEKMHOPCTYX".toLowerCase()
    var region = numb.slice(6)
    if (9 - le != 0) {
        var temp = ""
        for (i = 0; i < (9 - le); i++) {
            temp += "0"
        }
        var newRegion = temp + region
    }
    else {
        var newRegion = region
    }
    var letNumb = (numb[0].toString().toLowerCase() + numb.slice(4, 6).toLowerCase())
    var intNumb = numb.slice(1, 4)

    if ((/^\d+$/.test(intNumb) == false) || (!allCharsIn(letNumb, allowLetters)) || (/^\d+$/.test(region) == false)) {
        return ""
    }
    return letNumb[0].toString().toUpperCase() + intNumb + letNumb.slice(1, 3).toUpperCase() + newRegion
}

function allCharsIn(part, full) {
    return [...part].every(c => full.includes(c));
}

function buttonUsless() {
    $('button').attr('readonly', true);
    $('button').attr('disabled', true);
}