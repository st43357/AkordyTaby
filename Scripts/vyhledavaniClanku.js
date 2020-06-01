function vyhledavaniClanku() {
    // Declare variables
    var input, filter, table, mazanyContainer, div, i, txtValue;
    input = document.getElementById("myInput");
    filter = input.value.toUpperCase();
    table = document.getElementById("myTable");
    mazanyContainer = table.getElementsByTagName("div");

    // Loop through all table rows, and hide those who don't match the search query
    for (i = 0; i < mazanyContainer.length; i++) {
        div = mazanyContainer[i].getElementsByTagName("h2")[0];
        if (div) {
            txtValue = div.textContent || div.innerText;
            if (txtValue.toUpperCase().indexOf(filter) > -1) {
                mazanyContainer[i].style.display = "";
            } else {
                mazanyContainer[i].style.display = "none";
            }
        }
    }
}