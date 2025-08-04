function PhoneParsValid() {
    var phone = document.getElementById('phone_input').value;
    $.ajax({
        method: 'post',
        url: '/Login/TryNormalizePhone?maskedInput=' + phone,
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
            if (result != '') {
                $.toast({
                    text: "Код подтверждения отправлен",
                    showHideTransition: 'slide',
                    icon: 'success',
                    position: 'top-right',
                });

                document.getElementById('login').hidden = false;
                document.getElementById('code').hidden = false;
                document.getElementById('code_in').hidden = true;
            }
            else {
                $.toast({
                    text: 'Неверный формат',
                    showHideTransition: 'slide',
                    icon: 'error',
                    position: 'top-right',
                });
            }
        }
    });
}

function VerifyCode() {
    var code = document.getElementById('code_input').value;
    $.ajax({
        method: 'post',
        url: '/Login/VerifyCode?code=' + code,
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
                    text: "Успешный вход",
                    showHideTransition: 'slide',
                    icon: 'success',
                    position: 'top-right',
                });
                buttonUsless();
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
    });
}

function buttonUsless() {
    $('button').attr('readonly', true);
    $('button').attr('disabled', true);
}