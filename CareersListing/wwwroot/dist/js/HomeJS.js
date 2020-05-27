// get all the slider images
const sliderImage = document.querySelector(".slider-area");
const sliderTitle = document.querySelector(".slider-desc .title-wrap");
const imageNames = ["img1.png", "img3.png"];
const titles = ["Your smart choice", "Own the dream job"];
const animateStyles = ["fadeInLeft", "fadeIn", "fadeInUp"];

let counter = 1;
//alert(sliderTitles[0].innerHTML);

let intervals = window.setInterval(slideIt, 12500);
function slideIt() {
    if (counter > (imageNames.length - 1)) {
        counter = 0;
    }

    let animateImage = Math.floor(Math.random() * Math.floor(animateStyles.length - 1));
    let animateText = Math.floor(Math.random() * Math.floor(animateStyles.length - 1));

    const childImage = document.querySelector(".slider-area img");
    sliderImage.removeChild(childImage);
    const newImg = document.createElement("img");
    newImg.src = "images/slider/" + imageNames[counter];
    newImg.classList.add(animateStyles[animateImage]);
    newImg.classList.add("animated");
    newImg.classList.add("active");
    sliderImage.appendChild(newImg);

    const childTitle = document.querySelector(".slider-desc > .title-wrap .title");
    sliderTitle.removeChild(childTitle);
    const newTitle = document.createElement("div");
    newTitle.innerHTML = titles[counter];
    newTitle.classList.add(animateStyles[animateText]);
    newTitle.classList.add("animated");
    newTitle.classList.add("title");
    sliderTitle.appendChild(newTitle);

    counter += 1;
}
