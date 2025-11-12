

const mainDetails=document.querySelector(".movie-details")
const id = window.location.search.slice(4)
fetch(`https://api.tvmaze.com/shows/${id}`)
    .then(response => response.json())
    .then(movie => {
       mainDetails.innerHTML += `
    <img src="${movie.image.original}" alt="${movie.name}">
    <div class="movie-info">
        <h1 class="movie-name">${movie.name}</h1>
        <p class="movie-genres">${movie.genres.join(", ")}</p>
        <p class="movie-language">${movie.language}</p>
        <p class="movie-raiting">IMDb: ${movie.rating.average}</p>
        <p class="movie-summary">${movie.summary}</p>
        
    </div>`
    })
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