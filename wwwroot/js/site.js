/**
 * Blog System - Enhanced JavaScript
 * Provides interactive features and modern UI enhancements
 */

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    initializeComponents();
    setupFormValidation();
    setupInteractiveFeatures();
    setupAnimations();

/**
 * Initialize all components
 */
function initializeComponents() {
    // Initialize Bootstrap tooltips
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
    
    // Initialize Bootstrap popovers
    const popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });
    
    // Auto-hide alerts after 5 seconds
    setTimeout(function() {
        const alerts = document.querySelectorAll('.alert:not(.alert-permanent)');
        alerts.forEach(function(alert) {
            if (bootstrap.Alert.getInstance(alert)) {
                bootstrap.Alert.getInstance(alert).close();
            }
        });
    }, 5000);
}

/**
 * Enhanced form validation with smooth animations
 */
function setupFormValidation() {
    const forms = document.querySelectorAll('form[data-enhanced-validation="true"]');
    
    forms.forEach(function(form) {
        form.addEventListener('submit', function(e) {
            let isValid = true;
            const invalidFields = [];
            
            // Check all required fields
            const requiredFields = form.querySelectorAll('input[required], textarea[required], select[required]');
            
            requiredFields.forEach(function(field) {
                if (!field.value.trim()) {
                    field.classList.add('is-invalid');
                    field.classList.remove('is-valid');
                    invalidFields.push(field);
                    isValid = false;
                } else {
                    field.classList.add('is-valid');
                    field.classList.remove('is-invalid');
                }
            });
            
            // Check email fields
            const emailFields = form.querySelectorAll('input[type="email"]');
            emailFields.forEach(function(field) {
                if (field.value && !isValidEmail(field.value)) {
                    field.classList.add('is-invalid');
                    field.classList.remove('is-valid');
                    invalidFields.push(field);
                    isValid = false;
                }
            });
            
            if (!isValid) {
                e.preventDefault();
                // Smooth scroll to first invalid field
                if (invalidFields.length > 0) {
                    invalidFields[0].scrollIntoView({ 
                        behavior: 'smooth', 
                        block: 'center' 
                    });
                    invalidFields[0].focus();
                }
                
                // Show error notification
                showNotification('Please fix the errors below', 'error');
            }
        });
        
        // Real-time validation
        const inputs = form.querySelectorAll('input, textarea, select');
        inputs.forEach(function(input) {
            input.addEventListener('blur', function() {
                validateField(input);
            });
            
            input.addEventListener('input', function() {
                if (input.classList.contains('is-invalid')) {
                    validateField(input);
                }
            });
        });
    });
}

/**
 * Validate individual field
 */
function validateField(field) {
    const value = field.value.trim();
    let isValid = true;
    
    if (field.hasAttribute('required') && !value) {
        isValid = false;
    }
    
    if (field.type === 'email' && value && !isValidEmail(value)) {
        isValid = false;
    }
    
    if (isValid) {
        field.classList.add('is-valid');
        field.classList.remove('is-invalid');
    } else {
        field.classList.add('is-invalid');
        field.classList.remove('is-valid');
    }
    
    return isValid;
}

/**
 * Email validation helper
 */
function isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

/**
 * Setup interactive features
 */
function setupInteractiveFeatures() {
    // Search functionality with debouncing
    const searchInputs = document.querySelectorAll('input[data-search="true"]');
    searchInputs.forEach(function(input) {
        let timeout;
        input.addEventListener('input', function() {
            clearTimeout(timeout);
            timeout = setTimeout(function() {
                performSearch(input.value);
            }, 300);
        });
    });
    
    // Dynamic loading indicators
    const submitButtons = document.querySelectorAll('button[type="submit"]');
    submitButtons.forEach(function(button) {
        button.addEventListener('click', function() {
            if (button.form && button.form.checkValidity()) {
                showLoadingState(button);
            }
        });
    });
    
    // Confirm dialogs for dangerous actions
    const dangerButtons = document.querySelectorAll('[data-confirm]');
    dangerButtons.forEach(function(button) {
        button.addEventListener('click', function(e) {
            const message = button.getAttribute('data-confirm');
            if (!confirm(message)) {
                e.preventDefault();
            }
        });
    });
    
    // Auto-expand textareas
    const textareas = document.querySelectorAll('textarea[data-auto-expand="true"]');
    textareas.forEach(function(textarea) {
        textarea.addEventListener('input', function() {
            autoExpand(textarea);
        });
        autoExpand(textarea); // Initialize
    });
    
    // Character counters
    const fieldsWithCounters = document.querySelectorAll('[data-max-length]');
    fieldsWithCounters.forEach(function(field) {
        const maxLength = parseInt(field.getAttribute('data-max-length'));
        const counter = document.createElement('small');
        counter.className = 'text-muted character-counter';
        field.parentNode.appendChild(counter);
        
        function updateCounter() {
            const remaining = maxLength - field.value.length;
            counter.textContent = `${remaining} characters remaining`;
            
            if (remaining < 10) {
                counter.className = 'text-danger character-counter';
            } else if (remaining < 50) {
                counter.className = 'text-warning character-counter';
            } else {
                counter.className = 'text-muted character-counter';
            }
        }
        
        field.addEventListener('input', updateCounter);
        updateCounter();
    });
    
    // Comment reply functionality
    const replyButtons = document.querySelectorAll('.reply-btn');
    replyButtons.forEach(function(button) {
        button.addEventListener('click', function() {
            const commentId = button.getAttribute('data-comment-id');
            const author = button.getAttribute('data-author');
            showReplyForm(commentId, author);
        });
    });
}

/**
 * Setup animations and visual enhancements
 */
function setupAnimations() {
    // Intersection Observer for scroll animations
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };
    
    const observer = new IntersectionObserver(function(entries) {
        entries.forEach(function(entry) {
            if (entry.isIntersecting) {
                entry.target.classList.add('fade-in-up');
                observer.unobserve(entry.target);
            }
        });
    }, observerOptions);
    
    // Observe elements for animation
    const animateElements = document.querySelectorAll('.card, .blog-post-card, .admin-stats-card');
    animateElements.forEach(function(el) {
        observer.observe(el);
    });
    
    // Smooth hover effects for cards
    const cards = document.querySelectorAll('.card');
    cards.forEach(function(card) {
        card.addEventListener('mouseenter', function() {
            card.style.transform = 'translateY(-4px)';
        });
        
        card.addEventListener('mouseleave', function() {
            card.style.transform = 'translateY(0)';
        });
    });
}

/**
 * Show loading state on button
 */
function showLoadingState(button) {
    const originalText = button.innerHTML;
    button.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Processing...';
    button.disabled = true;
    
    // Reset after 10 seconds as fallback
    setTimeout(function() {
        button.innerHTML = originalText;
        button.disabled = false;
    }, 10000);
}

/**
 * Auto-expand textarea based on content
 */
function autoExpand(textarea) {
    textarea.style.height = 'auto';
    textarea.style.height = textarea.scrollHeight + 'px';
}

/**
 * Show notification toast
 */
function showNotification(message, type = 'info') {
    const toastContainer = getOrCreateToastContainer();
    const toastId = 'toast-' + Date.now();
    
    const toastHtml = `
        <div id="${toastId}" class="toast align-items-center text-white bg-${type === 'error' ? 'danger' : type}" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="d-flex">
                <div class="toast-body">
                    <i class="fas fa-${getIconForType(type)} me-2"></i>
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>
    `;
    
    toastContainer.insertAdjacentHTML('beforeend', toastHtml);
    const toastElement = document.getElementById(toastId);
    const toast = new bootstrap.Toast(toastElement);
    toast.show();
    
    // Remove element after it's hidden
    toastElement.addEventListener('hidden.bs.toast', function() {
        toastElement.remove();
    });
}

/**
 * Get or create toast container
 */
function getOrCreateToastContainer() {
    let container = document.querySelector('.toast-container');
    if (!container) {
        container = document.createElement('div');
        container.className = 'toast-container position-fixed top-0 end-0 p-3';
        document.body.appendChild(container);
    }
    return container;
}

/**
 * Get icon for notification type
 */
function getIconForType(type) {
    const icons = {
        success: 'check-circle',
        error: 'exclamation-triangle',
        warning: 'exclamation-circle',
        info: 'info-circle'
    };
    return icons[type] || 'bell';
}

/**
 * Show reply form for comment
 */
function showReplyForm(commentId, author) {
    // Remove any existing reply forms
    const existingForms = document.querySelectorAll('.reply-form');
    existingForms.forEach(form => form.remove());
    
    const replyFormHtml = `
        <div class="reply-form mt-3 p-3 bg-light rounded">
            <h6>Reply to ${author}</h6>
            <form>
                <input type="hidden" name="ParentCommentId" value="${commentId}">
                <div class="mb-3">
                    <textarea class="form-control" name="Content" rows="3" placeholder="Write your reply..." required></textarea>
                </div>
                <div class="d-flex gap-2">
                    <button type="submit" class="btn btn-primary btn-sm">
                        <i class="fas fa-reply me-1"></i>Post Reply
                    </button>
                    <button type="button" class="btn btn-secondary btn-sm cancel-reply">
                        <i class="fas fa-times me-1"></i>Cancel
                    </button>
                </div>
            </form>
        </div>
    `;
    
    const commentElement = document.getElementById(`comment-${commentId}`);
    commentElement.insertAdjacentHTML('beforeend', replyFormHtml);
    
    // Focus on textarea
    const textarea = commentElement.querySelector('.reply-form textarea');
    textarea.focus();
    
    // Handle cancel button
    const cancelBtn = commentElement.querySelector('.cancel-reply');
    cancelBtn.addEventListener('click', function() {
        commentElement.querySelector('.reply-form').remove();
    });
}

/**
 * Perform search (placeholder for actual search implementation)
 */
function performSearch(query) {
    console.log('Searching for:', query);
    // This would typically make an AJAX request to search endpoint
}

// Utility functions for backward compatibility
window.showNotification = showNotification;
window.showLoadingState = showLoadingState;

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
