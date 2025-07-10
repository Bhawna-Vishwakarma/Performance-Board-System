

document.querySelectorAll('.counter').forEach(counter => {
    counter.innerText = '0';
    const updateCounter = () => {
        const target = +counter.innerText;
        const newVal = +counter.getAttribute('data-target') || +counter.innerText;
        const increment = Math.ceil(newVal / 50);

        if (target < newVal) {
            counter.innerText = `${Math.min(target + increment, newVal)}`;
            setTimeout(updateCounter, 50);
        } else {
            counter.innerText = newVal;
        }
    };
    updateCounter();
});
