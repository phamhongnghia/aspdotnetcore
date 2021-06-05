$(document).ready(function() {
	Effect();

	let listRoot = document.getElementsByClassName("root__price");
	let discounted = document.getElementsByClassName("discounted");
	for (let i of listRoot) {
		i.innerText = fomatter.format(i.innerText);
	}
	for (let i of discounted) {
		i.innerText = fomatter.format(i.innerText);
	}
});
function Effect(){
	window.addEventListener("scroll",function(){
		var header = document.querySelector('.header__sticky');
		header.classList.toggle("sticky",window.scrollY > 0);
	});
	$('.menu__dropdown').hover(function () {
		$(this).css({
			'opacity': '1',
			'visibility': 'visible',
			'top': '100%'
		})
	}, function () {
		$(this).removeAttr('style');
	});
	$('.list__sale').slick({
		slidesToShow: 4,
		infinite: false
	});
	$('.related__product').slick({
		slidesToShow: 3,
		infinite: false
	});
	$('.menu__product a').click(function() {
		$('.menu__product a').removeClass('active__menu');
		$(this).addClass('active__menu');
	});
}