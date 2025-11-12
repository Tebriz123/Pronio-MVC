
const row = document.querySelector("#movies")
const leftBtn = document.querySelector(".left-btn");
const rightBtn = document.querySelector(".right-btn");
fetch("https://api.tvmaze.com/shows")
    .then(response => response.json())
    .then(movies => {
      const row = document.querySelector("#movies");

fetch("https://api.tvmaze.com/shows")
  .then(response => response.json())
  .then(movies => {
    movies.forEach(movie => {
      row.innerHTML += `
        <div class="card">
          <img src="${movie.image.medium}" class="card-img-top" alt="${movie.name}">
          <div class="card-body">
            <h5 class="card-title">${movie.name}</h5>
            <p class="card-text">${movie.rating.average ?? "N/A"}</p>
            <a href="details.html?id=${movie.id}" class="btn btn-primary">Go to details</a>
          </div>
        </div>
      `;
    });
  });

    })

    rightBtn.addEventListener("click", () => {
  row.scrollBy({ left: 400, behavior: "smooth" });
});

leftBtn.addEventListener("click", () => {
  row.scrollBy({ left: -400, behavior: "smooth" });
});

    const myCarousel = document.querySelector('#exxenSlider');
const carousel = new bootstrap.Carousel(myCarousel, {
  interval: 3000,
  ride: 'carousel'
});


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