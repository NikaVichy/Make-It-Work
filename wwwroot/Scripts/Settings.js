document.querySelectorAll('.choice').forEach(initChoice);

function initChoice(choice) {
  const displayer = choice.querySelector('.choice-displayer');
  const form = choice.querySelector('.choice-form');

  if (!displayer || !form) return;

  function open() {
    choice.classList.add('open');
    form.style.maxHeight = form.scrollHeight + 'px';
    form.setAttribute('aria-hidden', 'false');
    choice.setAttribute('aria-expanded', 'true');
  }

  function close() {
    choice.classList.remove('open');
    form.style.maxHeight = '0';
    form.setAttribute('aria-hidden', 'true');
    choice.setAttribute('aria-expanded', 'false');
  }

  function toggle() {
    choice.classList.contains('open') ? close() : open();
  }

  displayer.addEventListener('click', toggle);
  displayer.addEventListener('keydown', (e) => {
    if (e.key === 'Enter' || e.key === ' ') {
      e.preventDefault();
      toggle();
    }
  });

  form.addEventListener('click', e => e.stopPropagation());

  // при ресайзе пересчитываем высоту для открытых форм
  window.addEventListener('resize', () => {
    if (choice.classList.contains('open')) {
      form.style.maxHeight = form.scrollHeight + 'px';
    }
  });
}