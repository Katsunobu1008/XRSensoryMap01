const filters = [
  '眩しい刺激',
  '混雑刺激',
  'におい刺激',
  '音刺激',
  '休憩場所',
  '暗い場所',
  '静かな場所',
];

document.addEventListener('DOMContentLoaded', () => {
  createFilterButtons();
  setupModalListeners();
  setupStartButton();
});

function createFilterButtons() {
  const filtersContainer = document.querySelector('.filters');
  filters.forEach((filter) => {
    const button = document.createElement('button');
    button.textContent = filter;
    button.classList.add('filter-button');
    button.addEventListener('click', () => toggleFilter(button, filter));
    filtersContainer.appendChild(button);
  });
}

function toggleFilter(button, filter) {
  button.classList.toggle('active');
  console.log(
    `Filter ${filter} is ${
      button.classList.contains('active') ? 'active' : 'inactive'
    }`
  );
}

function setupModalListeners() {
  const modal = document.getElementById('modal');
  const closeBtn = document.querySelector('.close');
  closeBtn.onclick = () => (modal.style.display = 'none');
  window.onclick = (event) => {
    if (event.target == modal) {
      modal.style.display = 'none';
    }
  };
}

function setupStartButton() {
  const startButton = document.getElementById('start-button');
  startButton.addEventListener('click', () => {
    console.log('Loading Unity WebGL content...');
    var unityInstance = UnityLoader.instantiate(
      'unity-canvas',
      'unity/Build/your_build_name.json',
      {
        onProgress: UnityProgress,
      }
    );
  });
}

function UnityProgress(unityInstance, progress) {
  if (progress === 1) {
    console.log('Unity WebGL content loaded successfully');
  }
}

function onUnityObjectClicked(objectData) {
  const modal = document.getElementById('modal');
  document.getElementById('modal-title').textContent = objectData.title;
  document.getElementById('modal-description').textContent =
    objectData.description;
  const imagesContainer = document.getElementById('modal-images');
  imagesContainer.innerHTML = '';
  objectData.images.forEach((imageUrl) => {
    const img = document.createElement('img');
    img.src = imageUrl;
    img.alt = objectData.title;
    imagesContainer.appendChild(img);
  });
  modal.style.display = 'block';
}
