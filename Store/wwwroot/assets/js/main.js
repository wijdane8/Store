(function () {
    "use strict";

    /* ---------------------------
     Global Cart Counter System
    ---------------------------*/
    window.updateCartCount = async function () {
        const cartElements = document.querySelectorAll('.cart-count');
        const isAuthenticated = document.body.dataset.loggedIn === "true";
        const currentCount = sessionStorage.getItem('cartCount') || '0';

        // Immediate UI update with session storage value
        cartElements.forEach(el => el.textContent = currentCount);

        if (!isAuthenticated) {
            sessionStorage.removeItem('cartCount');
            return;
        }

        try {
            // Cache-busting request
            const response = await fetch(`/Cart/GetCount?_=${Date.now()}`, {
                cache: 'no-cache',
                headers: {
                    'Content-Type': 'application/json',
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            if (!response.ok) throw new Error(`HTTP ${response.status}`);
            if (response.headers.get('content-type')?.includes('application/json')) {
                const data = await response.json();
                const newCount = data.count.toString();

                // Update UI and storage only if changed
                if (newCount !== currentCount) {
                    cartElements.forEach(el => {
                        el.textContent = newCount;
                        el.classList.add('count-updated');
                        setTimeout(() => el.classList.remove('count-updated'), 300);
                    });
                    sessionStorage.setItem('cartCount', newCount);
                }
            }
        } catch (error) {
            console.error('Cart counter error:', error);
            sessionStorage.removeItem('cartCount');
            cartElements.forEach(el => el.textContent = '0');
        }
    };

    /* ---------------------------
     Core Functionality
    ---------------------------*/
    
    // Scroll Handlers
    function handleScroll() {
        // Header scroll class
        document.body.classList.toggle('scrolled', window.scrollY > 100);
        
        // Scroll top button
        const scrollTop = document.querySelector('.scroll-top');
        if (scrollTop) {
            scrollTop.classList.toggle('active', window.scrollY > 100);
        }
    }

    // Mobile Navigation
    function initMobileNav() {
        const mobileNavToggle = document.querySelector('.mobile-nav-toggle');
        if (!mobileNavToggle) return;

        mobileNavToggle.addEventListener('click', () => {
            document.body.classList.toggle('mobile-nav-active');
            mobileNavToggle.classList.toggle('bi-x');
            mobileNavToggle.setAttribute(
                'aria-expanded', 
                document.body.classList.contains('mobile-nav-active')
            );
        });

        // Close mobile nav on click
        document.querySelectorAll('#navmenu a').forEach(link => {
            link.addEventListener('click', () => {
                if (document.body.classList.contains('mobile-nav-active')) {
                    document.body.classList.remove('mobile-nav-active');
                    mobileNavToggle.classList.remove('bi-x');
                    mobileNavToggle.setAttribute('aria-expanded', 'false');
                }
            });
        });
    }

    /* ---------------------------
     Initialization
    ---------------------------*/
    function init() {
        // Initial cart count
        window.updateCartCount();
        
        // Auto-refresh cart every 30 seconds
        setInterval(window.updateCartCount, 30000);

        // Scroll handlers
        window.addEventListener('scroll', handleScroll, { passive: true });
        handleScroll(); // Initial check

        // Mobile navigation
        initMobileNav();

        // Initialize AOS
        if (typeof AOS !== 'undefined') {
            AOS.init({
                duration: 600,
                easing: 'ease-in-out',
                once: true,
                mirror: false,
                offset: 100
            });
        }

        // Scroll top button
        document.querySelector('.scroll-top')?.addEventListener('click', (e) => {
            e.preventDefault();
            window.scrollTo({
                top: 0,
                behavior: 'smooth'
            });
        });

        // Initialize GLightbox
        if (typeof GLightbox !== 'undefined') {
            const glightbox = GLightbox({ selector: '.glightbox' });
        }

        // Initialize PureCounter
        if (typeof PureCounter !== 'undefined') {
            new PureCounter();
        }
    }

    // Start everything
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

})();