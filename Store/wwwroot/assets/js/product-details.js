// product-details.js
document.addEventListener("DOMContentLoaded", function () {
    // Initialize animations
    AOS.init({
        duration: 800,
        easing: "ease-in-out",
        once: true,
    });

    // Initialize Bootstrap tooltips
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // DOM elements
    const addToCartButton = document.getElementById('addToCart');
    const buyNowButton = document.getElementById('buyNow');
    const addToFavoritesButton = document.getElementById('addToFavorites');
    const notifyMeButton = document.getElementById('notifyMe');
    const addToCompareButton = document.getElementById('addToCompare');
    const quantityInput = document.querySelector('.quantity-input');
    const quantityMinus = document.querySelector('.quantity-minus');
    const quantityPlus = document.querySelector('.quantity-plus');
    const quantityError = document.getElementById('quantity-error');
    const shareButton = document.querySelector('.share-btn');
    const writeReviewButton = document.querySelector('.write-review-btn');
    const reviewForm = document.querySelector('.review-form');
    const ratingStars = document.querySelectorAll('.rating-input i');
    const productRatingInput = document.getElementById('productRating');
    const submitReviewForm = document.getElementById('submitReviewForm');
    const thumbnailImages = document.querySelectorAll('.thumbnail-gallery .img-thumbnail');
    const mainProductImage = document.getElementById('mainProductImage');
    const loginModal = new bootstrap.Modal(document.getElementById('loginModal'));

    // Helper function to show toast notifications
    function showToast(message, type = 'success') {
        // Remove existing toasts
        const existingToasts = document.querySelectorAll('.toast');
        existingToasts.forEach(toast => toast.remove());

        const toast = document.createElement('div');
        toast.className = `toast show align-items-center text-white bg-${type}`;
        toast.setAttribute('role', 'alert');
        toast.setAttribute('aria-live', 'assertive');
        toast.setAttribute('aria-atomic', 'true');
        toast.style.position = 'fixed';
        toast.style.bottom = '20px';
        toast.style.right = '20px';
        toast.style.zIndex = '9999';
        toast.style.minWidth = '250px';

        toast.innerHTML = `
            <div class="d-flex">
                <div class="toast-body">${message}</div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        `;

        document.body.appendChild(toast);

        // Auto dismiss after 3 seconds
        setTimeout(() => {
            toast.classList.add('hide');
            setTimeout(() => toast.remove(), 300);
        }, 3000);
    }

    // Quantity input validation
    function validateQuantity() {
        if (!quantityInput) return true;

        const quantity = parseInt(quantityInput.value);
        const maxQuantity = parseInt(quantityInput.max);

        if (isNaN(quantity)) {
            quantityError.textContent = 'الرجاء إدخال كمية صحيحة';
            quantityError.style.display = 'block';
            return false;
        }

        if (quantity < 1) {
            quantityInput.value = 1;
        } else if (quantity > maxQuantity) {
            quantityError.textContent = `الكمية المتوفرة هي ${maxQuantity} فقط`;
            quantityError.style.display = 'block';
            return false;
        }

        quantityError.style.display = 'none';
        return true;
    }

    // Quantity controls
    if (quantityMinus && quantityPlus && quantityInput) {
        quantityMinus.addEventListener('click', () => {
            let value = parseInt(quantityInput.value);
            if (value > 1) {
                quantityInput.value = value - 1;
                validateQuantity();
            }
        });

        quantityPlus.addEventListener('click', () => {
            let value = parseInt(quantityInput.value);
            if (value < parseInt(quantityInput.max)) {
                quantityInput.value = value + 1;
                validateQuantity();
            }
        });

        quantityInput.addEventListener('input', () => {
            validateQuantity();
        });

        quantityInput.addEventListener('blur', () => {
            if (quantityInput.value === '') {
                quantityInput.value = 1;
            }
            validateQuantity();
        });
    }

    // Add to cart functionality
    if (addToCartButton) {
        addToCartButton.addEventListener('click', async () => {
            if (!validateQuantity()) return;

            const quantity = parseInt(quantityInput.value);
            const productId = addToCartButton.dataset.productId;
            const productName = '@Model.Name';

            try {
                // Show loading state
                addToCartButton.disabled = true;
                addToCartButton.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> جاري الإضافة...';

                // In a real implementation, you would make an API call here
                // Example:
                // const response = await fetch('/api/cart/add', {
                //     method: 'POST',
                //     headers: {
                //         'Content-Type': 'application/json',
                //         'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                //     },
                //     body: JSON.stringify({ productId, quantity })
                // });
                //
                // if (!response.ok) throw new Error('Failed to add to cart');

                // Simulate API call delay
                await new Promise(resolve => setTimeout(resolve, 1000));

                showToast(`تم إضافة ${quantity} من ${productName} إلى السلة`);

                // Update cart count in header (if exists)
                const cartCountElement = document.querySelector('.cart-count');
                if (cartCountElement) {
                    const currentCount = parseInt(cartCountElement.textContent) || 0;
                    cartCountElement.textContent = currentCount + quantity;
                    cartCountElement.classList.remove('d-none');
                }
            } catch (error) {
                console.error('Error adding to cart:', error);
                showToast('حدث خطأ أثناء إضافة المنتج إلى السلة', 'danger');
            } finally {
                // Reset button state
                addToCartButton.disabled = false;
                addToCartButton.innerHTML = '<i class="bi bi-cart-plus"></i> أضف إلى السلة';
            }
        });
    }

    // Buy now functionality
    if (buyNowButton) {
        buyNowButton.addEventListener('click', async () => {
            if (!validateQuantity()) return;

            const quantity = parseInt(quantityInput.value);
            const productId = addToCartButton.dataset.productId;

            try {
                // Show loading state
                buyNowButton.disabled = true;
                buyNowButton.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> جاري التوجيه...';

                // In a real implementation, you would:
                // 1. Add to cart (or create temporary cart)
                // 2. Redirect to checkout page

                // Simulate API call delay
                await new Promise(resolve => setTimeout(resolve, 800));

                // Redirect to checkout
                window.location.href = `/Checkout?productId=${productId}&quantity=${quantity}`;
            } catch (error) {
                console.error('Error in buy now:', error);
                showToast('حدث خطأ أثناء محاولة الشراء', 'danger');
                buyNowButton.disabled = false;
                buyNowButton.innerHTML = '<i class="bi bi-lightning"></i> شراء الآن';
            }
        });
    }

    // Add to favorites functionality
    if (addToFavoritesButton) {
        addToFavoritesButton.addEventListener('click', async () => {
            // Check if user is authenticated
            if (addToFavoritesButton.classList.contains('require-login')) {
                loginModal.show();
                return;
            }

            const productId = '@Model.Id';
            const productName = '@Model.Name';
            const isFavorite = addToFavoritesButton.classList.contains('active');

            try {
                // Show loading state
                addToFavoritesButton.disabled = true;

                // In a real implementation, you would make an API call here
                // Example:
                // const endpoint = isFavorite ? '/api/favorites/remove' : '/api/favorites/add';
                // const response = await fetch(endpoint, {
                //     method: 'POST',
                //     headers: {
                //         'Content-Type': 'application/json',
                //         'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                //     },
                //     body: JSON.stringify({ productId })
                // });
                //
                // if (!response.ok) throw new Error('Failed to update favorites');

                // Simulate API call delay
                await new Promise(resolve => setTimeout(resolve, 800));

                if (isFavorite) {
                    addToFavoritesButton.classList.remove('active', 'btn-warning');
                    addToFavoritesButton.classList.add('btn-outline-warning');
                    addToFavoritesButton.innerHTML = '<i class="bi bi-heart"></i> المفضلة';
                    showToast(`تم إزالة ${productName} من المفضلة`);
                } else {
                    addToFavoritesButton.classList.add('active', 'btn-warning');
                    addToFavoritesButton.classList.remove('btn-outline-warning');
                    addToFavoritesButton.innerHTML = '<i class="bi bi-heart-fill"></i> في المفضلة';
                    showToast(`تم إضافة ${productName} إلى المفضلة`);
                }
            } catch (error) {
                console.error('Error updating favorites:', error);
                showToast('حدث خطأ أثناء تحديث المفضلة', 'danger');
            } finally {
                addToFavoritesButton.disabled = false;
            }
        });
    }

    // Notify me functionality
    if (notifyMeButton) {
        notifyMeButton.addEventListener('click', async () => {
            const productId = '@Model.Id';
            const productName = '@Model.Name';

            try {
                // Show loading state
                notifyMeButton.disabled = true;
                notifyMeButton.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> جاري الإرسال...';

                // In a real implementation, you would make an API call here
                // Example:
                // const response = await fetch('/api/notifications/subscribe', {
                //     method: 'POST',
                //     headers: {
                //         'Content-Type': 'application/json',
                //         'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                //     },
                //     body: JSON.stringify({ productId })
                // });
                //
                // if (!response.ok) throw new Error('Failed to subscribe for notifications');

                // Simulate API call delay
                await new Promise(resolve => setTimeout(resolve, 800));

                notifyMeButton.innerHTML = '<i class="bi bi-bell-fill"></i> سأخبرك عند التوفر';
                showToast(`سيتم إعلامك عند توفر ${productName}`, 'info');
            } catch (error) {
                console.error('Error subscribing for notifications:', error);
                showToast('حدث خطأ أثناء التسجيل للإشعار', 'danger');
                notifyMeButton.disabled = false;
                notifyMeButton.innerHTML = '<i class="bi bi-bell"></i> أعلمني عند التوفر';
            }
        });
    }

    // Add to compare functionality
    if (addToCompareButton) {
        addToCompareButton.addEventListener('click', async () => {
            const productId = '@Model.Id';
            const productName = '@Model.Name';

            try {
                // Show loading state
                addToCompareButton.disabled = true;
                addToCompareButton.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> جاري الإضافة...';

                // In a real implementation, you would make an API call here
                // Example:
                // const response = await fetch('/api/compare/add', {
                //     method: 'POST',
                //     headers: {
                //         'Content-Type': 'application/json',
                //         'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                //     },
                //     body: JSON.stringify({ productId })
                // });
                //
                // if (!response.ok) throw new Error('Failed to add to compare');

                // Simulate API call delay
                await new Promise(resolve => setTimeout(resolve, 800));

                showToast(`تم إضافة ${productName} إلى قائمة المقارنة`, 'info');

                // Update compare count in header (if exists)
                const compareCountElement = document.querySelector('.compare-count');
                if (compareCountElement) {
                    const currentCount = parseInt(compareCountElement.textContent) || 0;
                    compareCountElement.textContent = currentCount + 1;
                    compareCountElement.classList.remove('d-none');
                }
            } catch (error) {
                console.error('Error adding to compare:', error);
                showToast('حدث خطأ أثناء إضافة المنتج للمقارنة', 'danger');
            } finally {
                addToCompareButton.disabled = false;
                addToCompareButton.innerHTML = '<i class="bi bi-arrow-left-right"></i> مقارنة';
            }
        });
    }

    // Share product functionality
    if (shareButton) {
        shareButton.addEventListener('click', async () => {
            const productName = '@Model.Name';
            const productUrl = window.location.href;

            try {
                if (navigator.share) {
                    await navigator.share({
                        title: productName,
                        text: `تحقق من ${productName} في متجرنا`,
                        url: productUrl,
                    });
                } else {
                    // Fallback for browsers that don't support Web Share API
                    const shareText = `${productName} - ${productUrl}`;
                    await navigator.clipboard.writeText(shareText);
                    showToast('تم نسخ رابط المنتج إلى الحافظة', 'info');
                }
            } catch (error) {
                console.error('Error sharing:', error);
                if (error.name !== 'AbortError') {
                    showToast('حدث خطأ أثناء مشاركة المنتج', 'danger');
                }
            }
        });
    }

    // Thumbnail image gallery functionality
    if (thumbnailImages.length > 0 && mainProductImage) {
        thumbnailImages.forEach(thumbnail => {
            thumbnail.addEventListener('click', () => {
                // Remove active class from all thumbnails
                thumbnailImages.forEach(img => img.classList.remove('active'));

                // Add active class to clicked thumbnail
                thumbnail.classList.add('active');

                // Update main image
                mainProductImage.src = thumbnail.src;
                mainProductImage.alt = thumbnail.alt;
            });
        });
    }

    // Review form functionality
    if (writeReviewButton && reviewForm) {
        writeReviewButton.addEventListener('click', () => {
            reviewForm.style.display = reviewForm.style.display === 'none' ? 'block' : 'none';
        });
    }

    // Star rating functionality
    if (ratingStars.length > 0 && productRatingInput) {
        ratingStars.forEach(star => {
            star.addEventListener('click', () => {
                const rating = parseInt(star.dataset.rating);
                productRatingInput.value = rating;

                // Update star display
                ratingStars.forEach((s, index) => {
                    if (index < rating) {
                        s.classList.add('bi-star-fill', 'active');
                        s.classList.remove('bi-star');
                    } else {
                        s.classList.add('bi-star');
                        s.classList.remove('bi-star-fill', 'active');
                    }
                });
            });

            star.addEventListener('mouseover', () => {
                const rating = parseInt(star.dataset.rating);

                // Preview hover state
                ratingStars.forEach((s, index) => {
                    if (index < rating) {
                        s.classList.add('bi-star-fill');
                        s.classList.remove('bi-star');
                    }
                });
            });

            star.addEventListener('mouseout', () => {
                const currentRating = parseInt(productRatingInput.value);

                // Revert to actual rating
                ratingStars.forEach((s, index) => {
                    if (index >= currentRating) {
                        s.classList.add('bi-star');
                        s.classList.remove('bi-star-fill');
                    }
                });
            });
        });
    }

    // Submit review form
    if (submitReviewForm) {
        submitReviewForm.addEventListener('submit', async (e) => {
            e.preventDefault();

            const reviewTitle = document.getElementById('reviewTitle').value;
            const reviewText = document.getElementById('reviewText').value;
            const rating = productRatingInput.value;

            if (rating === '0') {
                showToast('الرجاء اختيار تقييم', 'warning');
                return;
            }

            try {
                // Show loading state
                const submitButton = submitReviewForm.querySelector('button[type="submit"]');
                submitButton.disabled = true;
                submitButton.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> جاري الإرسال...';

// In a real implementation, you would make an API call here
// Example:
// const response = await fetch('/api/reviews', {
//     method: 'POST',
//     headers: {
//         'Content-Type': 'application/json',
//         'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
//     },
//     body: JSON.stringify