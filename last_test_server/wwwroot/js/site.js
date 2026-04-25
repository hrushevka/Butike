// Auto-dismiss alerts
setTimeout(function () {
    $('.alert').fadeOut('slow');
}, 5000);

// Form validation styles
$(document).ready(function () {
    $('.form-control').on('blur', function () {
        if ($(this).val().trim() !== '') {
            $(this).addClass('is-valid');
        } else {
            $(this).removeClass('is-valid');
        }
    });
});

// Confirm delete
function confirmDelete(message) {
    return confirm(message || 'Вы уверены?');
}

// Toggle password visibility
function togglePasswordVisibility(inputId, iconId) {
    var input = document.getElementById(inputId);
    var icon = document.getElementById(iconId);

    if (input.type === 'password') {
        input.type = 'text';
        icon.classList.remove('bi-eye');
        icon.classList.add('bi-eye-slash');
    } else {
        input.type = 'password';
        icon.classList.remove('bi-eye-slash');
        icon.classList.add('bi-eye');
    }
}