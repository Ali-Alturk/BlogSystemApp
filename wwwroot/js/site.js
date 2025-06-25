// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Enhanced form validation feedback
$(document).ready(function() {
    // Add Bootstrap validation classes
    $('form').on('submit', function() {
        var isValid = true;
        $(this).find('input[data-val="true"], textarea[data-val="true"], select[data-val="true"]').each(function() {
            var $this = $(this);
            var $feedback = $this.siblings('.invalid-feedback');
            var $validationSpan = $this.siblings('.text-danger');
            
            if ($this.valid && !$this.valid()) {
                $this.addClass('is-invalid').removeClass('is-valid');
                isValid = false;
            } else {
                $this.addClass('is-valid').removeClass('is-invalid');
            }
        });
        
        if (!isValid) {
            $('html, body').animate({
                scrollTop: $('.is-invalid').first().offset().top - 100
            }, 300);
        }
    });

    // Character counter for textareas
    $('textarea').each(function() {
        var $this = $(this);
        var maxLength = $this.attr('maxlength');
        
        if (maxLength) {
            var $counter = $('<small class="form-text text-muted">0 / ' + maxLength + ' characters</small>');
            $this.after($counter);
            
            $this.on('input', function() {
                var length = $this.val().length;
                $counter.text(length + ' / ' + maxLength + ' characters');
                
                if (length > maxLength * 0.9) {
                    $counter.removeClass('text-muted').addClass('text-warning');
                } else {
                    $counter.removeClass('text-warning').addClass('text-muted');
                }
            });
        }
    });

    // Auto-resize textareas
    $('textarea').on('input', function() {
        this.style.height = 'auto';
        this.style.height = (this.scrollHeight) + 'px';
    });

    // Search form enhancements
    $('input[name="SearchString"]').on('keyup', function(e) {
        if (e.keyCode === 13) {
            $(this).closest('form').submit();
        }
    });

    // Smooth scrolling for anchor links
    $('a[href^="#"]').on('click', function(event) {
        var target = $(this.getAttribute('href'));
        if (target.length) {
            event.preventDefault();
            $('html, body').stop().animate({
                scrollTop: target.offset().top - 100
            }, 300);
        }
    });

    // Confirm delete actions
    $('a[href*="/Delete"], button[type="submit"]').filter(function() {
        return $(this).text().toLowerCase().includes('delete');
    }).on('click', function(e) {
        if (!confirm('Are you sure you want to delete this item? This action cannot be undone.')) {
            e.preventDefault();
            return false;
        }
    });

    // Toast notifications (if needed)
    $('.alert').each(function() {
        var $alert = $(this);
        if ($alert.hasClass('alert-dismissible')) {
            setTimeout(function() {
                $alert.fadeOut();
            }, 5000);
        }
    });

    // Back to top button
    var $backToTop = $('<button id="back-to-top" class="btn btn-primary btn-floating position-fixed" style="bottom: 20px; right: 20px; display: none; z-index: 1000;"><i>↑</i></button>');
    $('body').append($backToTop);
    
    $(window).scroll(function() {
        if ($(this).scrollTop() > 300) {
            $backToTop.fadeIn();
        } else {
            $backToTop.fadeOut();
        }
    });
    
    $backToTop.click(function() {
        $('html, body').animate({scrollTop: 0}, 300);
        return false;
    });
});
