document.addEventListener('DOMContentLoaded', () => {
    var toastEl = document.getElementById('toast');
    var progressEl = document.getElementById('toastProgress');

    if (toastEl && progressEl) {
        var toast = new bootstrap.Toast(toastEl, { delay: 3000 });
        toast.show();

        progressEl.style.width = '100%'; // Start at full width

        var duration = 3000; // Total duration (3 seconds)
        var step = 333; // Interval step (10ms)
        var intervalCount = duration / step; // Total number of updates
        var decrement = 100 / intervalCount; // Amount to decrease per step

        var width = 100;
        var interval = setInterval(() => {
            width -= decrement;
            progressEl.style.width = width + '%';

            if (width <= 0) {
                clearInterval(interval);
                progressEl.style.width = '0%'; // Ensure it fully collapses
                toast.hide();
            }
        }, step);
    }
});