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

////export function initStreet() {
////    const panorama = new google.maps.StreetViewPanorama(
////        document.getElementById("pano"),
////        {
////            position: { lat: 42.345573, lng: -71.098326 },
////            addressControlOptions: {
////                position: google.maps.ControlPosition.BOTTOM_CENTER,
////            },
////            linksControl: false,
////            panControl: false,
////            enableCloseButton: false,
////        },
////    );
////}


export function openNav() {

	//Show the tapis table
	document.getElementById('tableTapis').style.visibility = 'visible';
	document.getElementById('SearchPremisePanel').style.visibility = 'hidden';
	
	const element = document.getElementById("buttonTapis");
	let pos = element.offsetLeft;

	document.getElementById("mySidenav").style.left = pos + "px";
	document.getElementById("mySidenav").style.width = "300px";

}

export function openNavSearch() {

	//hide the tapis filter
	document.getElementById('SearchPremisePanel').style.visibility = 'visible';
	document.getElementById('tableTapis').style.visibility = 'hidden';

	const element = document.getElementById("buttonTapis");
	let pos = element.offsetLeft;

	//get the screen width - the sidebar - 100px;
	let width = document.documentElement.clientWidth - 300;

	document.getElementById("mySidenav").style.left = pos + "px";
	document.getElementById("mySidenav").style.width = width + "px";
	//document.getElementById("mySidenav").style.width = "1000px";

}

export function closeNav() {
	document.getElementById("mySidenav").style.width = "0";
}