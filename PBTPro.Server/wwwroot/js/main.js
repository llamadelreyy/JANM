//export function openRightBar() {
//	document.querySelector('#infobox1').classList.toggle('open');
//	//addClass(panel, 'cd-panel--is-visible');
//	//alert('Hello');
//}

export function openRightBar() {
	//document.querySelector('#infobox1').classList.toggle('open');
	document.querySelector('#panel-right').classList.toggle('cd-panel--is-visible');
	//addClass(panel, 'cd-panel--is-visible');
	//alert('Hello');
}

export function openNav() {
	

	const element = document.getElementById("buttonTapis");
	let pos = element.offsetLeft;

	document.getElementById("mySidenav").style.left = pos + "px";
	document.getElementById("mySidenav").style.width = "300px";

}

export function closeNav() {
	document.getElementById("mySidenav").style.width = "0";
}