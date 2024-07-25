

document.addEventListener('DOMContentLoaded', (event) => {
    let sideBar = document.querySelector('.side-bar');
    let button = document.querySelector('#menu-btn');
    button.style.display = 'inline-block';
    handleResize();
    window.addEventListener('resize', handleResize);
    button.addEventListener('click', showSidebar);
});


function showSidebar() {
    let saidebar = document.querySelectorAll('.side-bar');
    var s = document.querySelectorAll('.Assign');

    if (saidebar[0].classList.contains('active')) {
        saidebar[0].classList.remove('active');
        if (window.innerWidth > 1200 && !saidebar[1].classList.contains('active'))
            saidebar[1].classList.add('active');

        s.forEach(function (element) {
            if (element.classList.contains('courses'))
                element.classList.remove('courses');;
        });
    }
    else {
        saidebar[0].classList.add('active');
        /* if (window.innerWidth > 1200 && !saidebar[1].classList.contains('active')) {
             saidebar[1].classList.add('active');
         }*/
        s.forEach(function (element) {
            if (!element.classList.contains('courses'))
                element.classList.add('courses');
        });
    }




}


function handleResize() {
    let sideBar = document.querySelectorAll('.side-bar');
    var s = document.querySelectorAll('.Assign');
    if (window.innerWidth < 1200) {

        sideBar.forEach(function (element) {
            if (element.classList.contains('active'))
                element.classList.remove('active');
        });


    }
    else {

        if (sideBar[0].classList.contains('active')) {
            sideBar[0].classList.remove('active');
            sideBar[1].classList.add('active');

        }
        else {
            sideBar[1].classList.add('active');
        }

        s.forEach(function (element) {
            if (element.classList.contains('courses'))
                element.classList.remove('courses');
        });

    }

}
function showDropdown() {

    let saidebar = document.querySelectorAll('.side-bar');
    saidebar[0].classList.add('active');
    saidebar[1].classList.remove('active');
}
