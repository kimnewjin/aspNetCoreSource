// Write your Javascript code.
function openPopup(pageUrl, width, height, windowName) {
    var feature = "width=" + width + ",height=" + height + ",scrollbars=yes,resizable=no,top=20,left=100,toolbar=no";
    window.open(pageUrl, windowName, feature);
}