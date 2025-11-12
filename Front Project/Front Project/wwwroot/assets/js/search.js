const row = document.querySelector("#movies");
const searchInput = document.querySelector("#searchInput");

searchInput.addEventListener("input", () => {
  const query = searchInput.value.trim();


  if (query === "") {
    fetch("https://api.tvmaze.com/shows")
      .then(res => res.json())
      .then(data => showMovies(data));
    return;
  }


  fetch(`https://api.tvmaze.com/search/shows?q=${query}`)
    .then(res => res.json())
    .then(data => {
      const searchResults = data.map(item => item.show);
      showMovies(searchResults);
    });
});

function showMovies(movies) {
  row.innerHTML = ""; 
  movies.forEach(movie => {
    row.innerHTML += `
      <div class="col-3">
        <div class="card" style="width: 18rem;">
          <img src="${movie.image ? movie.image.medium : 'https://via.placeholder.com/210x295?text=No+Image'}" class="card-img-top">
          <div class="card-body">
            <h5 class="card-title">${movie.name}</h5>
            <p class="card-text">${movie.genres.join(", ")}</p>
          </div>
        </div>
      </div>
    `;
  });
}

fetch("https://api.tvmaze.com/search/shows?q=bitten")
  .then(res => res.json())
  .then(data => showMovies(data));
  const langBtn = document.querySelector(".language-btn");
const langMenu = document.querySelector(".language-menu");
const langText = langBtn.querySelector("span");

langBtn.addEventListener("click", (e) => {
  e.stopPropagation();
  langMenu.classList.toggle("active");
});

langMenu.querySelectorAll("p").forEach(item => {
  item.addEventListener("click", () => {
    const selectedLang = item.getAttribute("data-lang");
    langText.textContent = selectedLang;
    langMenu.classList.remove("active");
  });
});

document.addEventListener("click", (e) => {
  if (!langBtn.contains(e.target) && !langMenu.contains(e.target)) {
    langMenu.classList.remove("active");
  }
});

const burgerBtn = document.getElementById("burger-btn");
const burgerMenu = document.getElementById("burger-menu");
const closeMenu = document.querySelector(".close-menu");

burgerBtn.addEventListener("click", () => {
  burgerMenu.classList.add("active");
});

closeMenu.addEventListener("click", () => {
  burgerMenu.classList.remove("active");
});

window.addEventListener("click", (e) => {
  if (e.target === burgerMenu) {
    burgerMenu.classList.remove("active");
  }
});