function messageCart(obj) {
	var mes = $(`
		<div class="message">
			<div class="message__img">
				<i class="fas fa-bell"></i>
			</div>
			<div class="message__text">
				${obj}
			</div>
		</div>
	`);
	$('.alert__cart').append(mes);
}