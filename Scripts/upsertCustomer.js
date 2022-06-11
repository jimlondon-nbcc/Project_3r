window.onload = function () {
    let state = document.querySelector("#Customer_State").value;
    if (state) {
        document.querySelector("#stateSelector").value = state;
    }
}