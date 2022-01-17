// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function openNav() {
    var mySidebar = document.getElementById("mySidebar");
    var content_area3_mySidebar = document.getElementById("content_area3_mySidebar");

    if (mySidebar.style.display === 'flex') {
        mySidebar.style.display = "none";
        content_area3_mySidebar.style.width = "100%";


    } else {
        mySidebar.style.display = 'flex'
        mySidebar.style.width = "calc(100 % / 6)";
        content_area3_mySidebar.style.width = "calc(100% - (100%/6)";
    }
}
