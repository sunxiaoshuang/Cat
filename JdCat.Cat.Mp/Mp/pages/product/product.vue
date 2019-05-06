<template>
	<view class="content">
		<mSearch @search="search($event)" :mode="2" :show="false" :placeholder="placeholder"></mSearch>
		<uni-collapse :accordion="true" v-if="productsFilter.length === 0">
			<uni-collapse-item :title="type.name" thumb="../../static/menu-list.png" v-for="type in types" v-bind:key="type.name">
				<uni-list>
					<uni-swipe-action :options="options" v-for="product in type.list" v-bind:key="product.name" @click="click($event, product)">
						<uni-list-item :title="product.name" :show-arrow="false" style="padding-left: 36upx;" :show-badge="product.status === 2" :badge-text="stateFilter(product.status)">
						</uni-list-item>
					</uni-swipe-action>
				</uni-list>
			</uni-collapse-item>
		</uni-collapse>
		<uni-list v-else>
			<uni-swipe-action :options="options" v-for="product in productsFilter" v-bind:key="product.name" @click="click($event, product)">
				<uni-list-item :title="product.name" :show-arrow="false" style="padding-left: 36upx;" :show-badge="product.status === 2" :badge-text="stateFilter(product.status)">
				</uni-list-item>
			</uni-swipe-action>
		</uni-list>



	</view>
</template>

<script>
	import mSearch from '@/components/mehaotian-search/mehaotian-search.vue'
	import uniList from '@/components/uni-list/uni-list.vue'
	import uniListItem from '@/components/uni-list-item/uni-list-item.vue'
	import uniCollapse from "@/components/uni-collapse/uni-collapse.vue"
	import uniCollapseItem from "@/components/uni-collapse-item/uni-collapse-item.vue"
	import uniSwipeAction from "@/components/uni-swipe-action/uni-swipe-action.vue"
    import helper from '../../common/helper.js';
	const stateArr = {0: '初始化', 1: '已上架', 2: '已下架', 3: '已删除'};
	export default {
		components: {
			mSearch,
			uniSwipeAction,
			uniList,
			uniListItem,
			uniCollapse,
			uniCollapseItem
		},
		data() {
			return {
				searchContent: '',
				placeholder: '搜索商品',
				types: [],
				products: [],
				productsFilter: [],
				options: [{
					text: '上架',
					style: {
						backgroundColor: '#007aff'
					}
				}, {
					text: '下架',
					style: {
						backgroundColor: '#dd524d'
					}
				}]
			}
		},
		onLoad(e) {
			var self = this;
			helper.request({
				url: "/mana/products/" + e.id,
				success(res){
					self.types = res.data;
					self.types.forEach(type => type.list.forEach(product => self.products.push(product)));
				}
			})
		},
		methods: {
			search(e) {
				if(!e) {
					this.productsFilter = [];
					return;
				}
				this.productsFilter = this.products.filter(product => product.name.indexOf(e) > -1);
			},
			click(e, product) {
				var status = e.text === "上架" ? 1 : 2;
				if(status === product.status)return;
				helper.request({
					url: `/mana/products/${product.id}?status=${status}`,
					method: "put",
					success(res){
						uni.showToast({
							title: e.text + '成功'
						});
						product.status = status;
					}
				})
			},
			stateFilter(state){
				return stateArr[state];
			}
		}
	}
</script>

<style>
	@import '../../static/fonts/iconfont.css';
</style>
