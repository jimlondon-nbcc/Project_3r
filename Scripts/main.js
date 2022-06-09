let CurrentPage = {
    baseUrl: '',
    searchKey: '',
    sortBy: 0,
    /**
     * Performs a search on table fields specified in Controller.
     **/
    Search() {
        this.searchKey = document.querySelector("#txtSearch").value;
        window.location.href = this.baseUrl + this.searchKey;
    },
    /**
     * Sorts table columns in ascending or descending order 
     * (using boolean "&isDesc" in URL string). 
     * @param {integer} sortBy The db column to sort by (see Controller for details)
     */
    Sort(sortBy) {
        let isDesc = false;

        let url_string = window.location.href;
        let url = new URL(url_string);
        let isDescUrlParam = url.searchParams.get("isDesc");

        if (isDescUrlParam == "false") {
            isDesc = true;
        }

        this.baseUrl = url.pathname;
        window.location.href = this.baseUrl + "?sortBy=" + sortBy + "&isDesc=" + isDesc;
    },
    /**
     * Gets rid of unsightly 12:00:00 AM in date elements. Can be called by any page
     * to remove timestamp from textboxes or other elements as long as class="date" is set.
     * @param {string} divider date divider (for US English use "/");
     */
    FixDates(divider) {
        let dates = document.querySelectorAll(".date");

        dates.forEach(d => {
            //For dates in an element 
            if (d.innerHTML) {
                let dateToFix = String(d.innerHTML);
                d.innerHTML = fixDateFormat(dateToFix);
            }
            //For dates in a textbox
            if (d.value) {
                let dateToFix = String(d.value);
                d.value = fixDateFormat(dateToFix);
            }
        });

        function fixDateFormat(dateToFix) {
            let dateParts = dateToFix.split(divider);

            let month = dateParts[0];
            let day = dateParts[1];
            let year = dateParts[2].substring(0, 4);

            let fixedDate = month + divider + day + divider + year;
            return fixedDate;
        }
    },
    /**
     * Checks page for fields with .upsertInfo class and stops Upsert
     * from going forward if they are not all filled
     * */
    Check() {
        let infoArray = document.querySelectorAll(".upsertInfo");

        let flag = true;

        for (let i = 0; i < infoArray.length; i++) {
            if (!infoArray[i].value) {
                flag = false;
                break;
            }
        }

        if (!flag) {
            alert("Please fill all required fields before clicking Submit.")
        }

        return flag;
    }
}
let api = {
    DELETE(url) {
        if (confirm("Are you sure you want to delete?")) {
            $.ajax({
                url: url,
                type: 'GET',
                success: function (res) {
                    debugger;
                }
            });
        } else {
            alert("Delete canceled.")
        }
    }
};

